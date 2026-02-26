import requests
import base64

# The URL of your local Flask server
url = "http://127.0.0.1:5000/assess"

# The path to your test image
image_path = r"C:\Users\joshu\Pictures\Castle.png"

try:
    # 1. Read the image and convert it to a Base64 string
    with open(image_path, "rb") as image_file:
        # We decode('utf-8') to turn the raw bytes into a standard string
        encoded_string = base64.b64encode(image_file.read()).decode('utf-8')

    print(f"Sending {image_path} to the AI...")

    # 2. Package it into a dictionary
    payload = {
        "image": encoded_string
    }

    # 3. Send the POST request
    # THE CRITICAL FIX: Using 'json=payload' automatically sets the Content-Type to 'application/json'
    response = requests.post(url, json=payload)

    # 4. Print the server's response
    print("\n--- Server Response ---")
    
    # Try to print it as a nicely formatted dictionary, otherwise print raw text
    try:
        print(response.json())
    except:
        print(response.text)

except FileNotFoundError:
    print(f"Error: Could not find the image at {image_path}")
except Exception as e:
    print(f"An error occurred: {e}")