# Stop console going nuts
import os  
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '1' 
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

from flask import Flask, request, jsonify
import base64
import numpy as np
import os
import io
from PIL import Image
import ollama

# Tensorflow Imports
import tensorflow as tf
from tensorflow.keras.applications import MobileNet
from tensorflow.keras.layers import Dense, Dropout
from tensorflow.keras.models import Model
from tensorflow.keras.applications.mobilenet import preprocess_input
from tensorflow.keras.preprocessing.image import img_to_array

import metrics 

app = Flask(__name__)

# Get the base folder path 
baseDir = os.path.dirname(os.path.abspath(__file__))

# Add base folder path to model location 
aestheticModelPath = os.path.join(baseDir, 'models', 'MobileNet', 'weights_mobilenet_aesthetic_0.07.hdf5')

def BuildNimaModel():
    print("Building MobileNet architecture...")
    # Load the base MobileNet (without the top classification layer)
    # pooling='avg' collapses the features into a single vector
    baseModel = MobileNet(input_shape=(224, 224, 3), include_top=False, pooling='avg', weights=None)
    
    # Add the NIMA specific layers (Dropout + 10-score Dense layer)
    # The 0.75 dropout is specific to the NIMA paper/implementation
    x = Dropout(0.75)(baseModel.output)
    x = Dense(10, activation='softmax')(x)
    
    # Create final model
    return Model(baseModel.input, x)

# Build nima model
nimaModel = BuildNimaModel()

# Load the weights into nimaModel
print(f"Loading weights from: {aestheticModelPath}")
try:
    nimaModel.load_weights(aestheticModelPath)
    print("SUCCESS: NIMA Model loaded and ready!")
except Exception as e:
    print(f"\nCRITICAL ERROR: Could not load weights.\n{e}")
    print("Double check your MODEL_PATH variable at the top of the script.")

def score_image(img_pil):
    # NIMA requires images to be 224x224
    img = img_pil.resize((224, 224))
    x = img_to_array(img)
    x = np.expand_dims(x, axis=0)
    x = preprocess_input(x)

    # Predict returns a distribution (probabilities for 1, 2, ... 10)
    scores = nimaModel.predict(x, batch_size=1, verbose=0)[0]

    # Calculate the mean score (Weighted average)
    mean_score = sum(scores[i] * (i + 1) for i in range(10))
    
    return float(mean_score)

@app.route('/ping', methods=['POST'])
def ping():
    # Grab the simple message sent from Unity
    incoming_message = request.form.get('message')
    
    # Print it to your Python console so you know it arrived
    print(f"Unity says: {incoming_message}")
    
    # Send a quick reply back to Unity
    return jsonify({
    "reply": "Flask hears you loud and clear!",
    "status_code": 200
    })

@app.route('/assess', methods=['POST'])
def assess_image():
    try:
        # Receive data from Unity
        # 'image_file' must match the name used in unity
        if 'image_file' not in request.files:
            return jsonify({"error": "No image file found in request"}), 400

        file = request.files['image_file']

        # Open the file stream directly into a PIL Image in memory
        pil_image = Image.open(file.stream).convert('RGB')

        # Run the Metrics (OpenCV)
        metric_results = metrics.analyze_image_metrics(pil_image)

        # Run the AI Aesthetic Model (NIMA)
        nima_score = score_image(pil_image)

        # Construct the Prompt for Ollama
        prompt = f"""You are a helpful virtual photography tutor inside a video game. 
        The student's in-game screenshot scored {nima_score}/10 on an aesthetic algorithm.

        Technical Analysis:
        - Exposure: {metric_results['brightness_comment']} 
        - Focus: {metric_results['blur_comment']} 
        - Composition: {metric_results['composition_comment']}

        Write a concise, 2-sentence encouraging critique. Do NOT mention real-world camera mechanics like 'aperture' or 'ISO'. Tell them how to improve using basic terms like 'move the camera', 'find better lighting', or 'center the subject' based on the technical analysis."""
        
        # Call the Local Ollama LLM
        # Ensure Ollama app is running and 'llama3.2:1b' is downloaded
        response = ollama.chat(model='llama3.2:1b', messages=[
            {
                'role': 'system',
                'content': 'You are a professional, encouraging photography instructor.'
            },
            {
                'role': 'user',
                'content': prompt
            }
        ])
        
        critique_text = response['message']['content']

        # Package everything into JSON and send it back to Unity
        return jsonify({
            "status": "success",
            "score": nima_score,
            "critique": critique_text,
            "metrics": metric_results
        }), 200

    except Exception as e:
        # If anything crashes, the server stays alive and tells Unity what went wrong
        print(f"Error processing image: {e}")
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    # Run the server on port 5000
    print("Starting Virtual Photography Server...")
    app.run(host='0.0.0.0', port=5000, debug=False)