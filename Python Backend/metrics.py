import cv2
import numpy as np

def analyze_image_metrics():
    # 1. Load Image
    img = cv2.imread(r'C:\Users\joshu\Pictures\Castle.png')
    if img is None:
        return "Error loading image"
    
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    height, width = gray.shape

    results = {
        "brightness_score": 0,
        "brightness_comment": "",
        "blur_score": 0,
        "blur_comment": "",
        "composition_score": 0,
        "composition_comment": ""
    }

    # --- METRIC 1: BRIGHTNESS (Exposure) ---
    # Calculate average pixel intensity (0-255)
    avg_brightness = np.mean(gray)
    results["brightness_score"] = round(avg_brightness, 2)
    
    if avg_brightness < 50:
        results["brightness_comment"] = "Underexposed (Too Dark)"
    elif avg_brightness > 200:
        results["brightness_comment"] = "Overexposed (Too Bright)"
    else:
        results["brightness_comment"] = "Good Exposure"

    # --- METRIC 2: BLUR (Focus) ---
    # Calculate the variance of the Laplacian (Edge detection)
    # Low variance = little edge detail = Blurry
    laplacian_var = cv2.Laplacian(gray, cv2.CV_64F).var()
    results["blur_score"] = round(laplacian_var, 2)

    if laplacian_var < 100:  # Threshold depends on image style, 100 is a good baseline
        results["blur_comment"] = "Blurry / Out of Focus"
    else:
        results["blur_comment"] = "Sharp Focus"

    # --- METRIC 3: COMPOSITION (Rule of Thirds via 'Center of Mass') ---
    # Threshold the image to find the "subject" (brightest parts)
    _, thresh = cv2.threshold(gray, 200, 255, cv2.THRESH_BINARY)
    
    # Calculate Moments to find the center of the 'subject'
    M = cv2.moments(thresh)
    if M["m00"] != 0:
        cX = int(M["m10"] / M["m00"])
        cY = int(M["m01"] / M["m00"])
        
        # Check X-axis Thirds (33% and 66%)
        third_1 = width / 3
        third_2 = (width / 3) * 2
        
        # Allow a buffer zone (e.g., +/- 5% of width)
        buffer = width * 0.05
        
        if (third_1 - buffer < cX < third_1 + buffer) or \
           (third_2 - buffer < cX < third_2 + buffer):
            results["composition_comment"] = "Strong Rule of Thirds Alignment"
        elif (width / 2 - buffer < cX < width / 2 + buffer):
             results["composition_comment"] = "Subject is Centered (Symmetric)"
        else:
            results["composition_comment"] = "Off-center composition"
    else:
        results["composition_comment"] = "No clear subject detected"

    return results