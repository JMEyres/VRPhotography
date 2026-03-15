import requests
import base64

# The URL of local Flask server
url = "http://127.0.0.1:5000/assess"

# path to test image
image_path = r"C:\Users\joshu\Pictures\Castle.png"

try:
    print(f"Sending {image_path} to the AI...")

    # Package into a dictionary
    payload = {
        "image_file": open(image_path, 'rb')
    }

    # Mimics unity output
    response = requests.post(url, files=payload)

    # Print the server response
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