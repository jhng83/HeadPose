# -*- coding: utf-8 -*-

import cv2
import mediapipe as mp
import numpy as np
import sys

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands
mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(min_detection_confidence=0.5, min_tracking_confidence=0.5)
cap = cv2.VideoCapture(0)
   
count = 0

while cap.isOpened():
    success, image = cap.read()
    count += 1

    # Flip the image horizontally for a later selfie-view display
    # Also convert the color space from BGR to RGB

    image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)
    scale_percent = 150 # percent of original size
    width = int(image.shape[1] * scale_percent / 100)
    height = int(image.shape[0] * scale_percent / 100)
    dim = (width, height)
    # resize image
    image = cv2.resize(image, dim, interpolation = cv2.INTER_AREA)
    with mp_hands.Hands(
        static_image_mode=True,
        max_num_hands=2,
        min_detection_confidence=0.5) as hands:
        results = hands.process(image)
    if results.multi_hand_landmarks:
        cv2.putText(image, "Recalibrating" , (30, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
        sys.stdout.flush()
        print("Recalibrating")
        #cv2.imshow('Head Pose Estimation', image)
    else:
        
        sys.stdout.flush()
        # To improve performance
        image.flags.writeable = False
        
        # Get the result
        results = face_mesh.process(image)
        
        # To improve performance
        image.flags.writeable = True
    
    
        img_h, img_w, img_c = image.shape
        face_3d = []
        face_2d = []
    
        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                for idx, lm in enumerate(face_landmarks.landmark):
                    if idx == 33 or idx == 263 or idx == 1 or idx == 61 or idx == 291 or idx == 199:
                        if idx == 1:
                            nose_2d = (lm.x * img_w, lm.y * img_h)
                            nose_3d = (lm.x * img_w, lm.y * img_h, lm.z * 8000)
    
                        x, y = int(lm.x * img_w), int(lm.y * img_h)
    
                        # 2D Coordinates
                        face_2d.append([x, y])
    
                        # 3D Coordinates
                        face_3d.append([x, y, lm.z])       
                
                # Convert to NumPy array
                face_2d = np.array(face_2d, dtype=np.float64)
                face_3d = np.array(face_3d, dtype=np.float64)
    
                # Set camera parameters
                focal_length = 1 * img_w
    
                cam_matrix = np.array([ [focal_length, 0, img_h / 2],
                                        [0, focal_length, img_w / 2],
                                        [0, 0, 1]])
   
                dist_matrix = np.zeros((4, 1), dtype=np.float64)
    
                # Perform SolvePnP using openCV
                success, rot_vec, trans_vec = cv2.solvePnP(face_3d, face_2d, cam_matrix, dist_matrix)
    
                # Get rotational matrix
                rmat, jac = cv2.Rodrigues(rot_vec)
                noseEndPoints3D = np.array([[0, 0, 1000.0]], dtype=np.float64)
                noseEndPoint2D, jacobian = cv2.projectPoints(
                   noseEndPoints3D, rot_vec, trans_vec, cam_matrix, dist_matrix)
    
                #  draw nose line
                p1 = (int(nose_2d[0]), int(nose_2d[1]))
                p2 = (int(noseEndPoint2D[0, 0, 0]), int(noseEndPoint2D[0, 0, 1]))
                cv2.line(image, p1, p2, (110, 220, 0),
                        thickness=2, lineType=cv2.LINE_AA)
    
                # Get angles
                angles, mtxR, mtxQ, Qx, Qy, Qz = cv2.RQDecomp3x3(rmat)
    
                # Get the pith and yaw
                pitch = str(round(angles[0] * 360,0)-5)
                yaw = str(round(angles[1] * 360,0))
               
                #Add the text on the image
                cv2.putText(image, pitch +","+ yaw , (30, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
                print("entry," + yaw +","+ pitch)
                
    scale_percent = 30 # percent of original size
    width = int(image.shape[1] * scale_percent / 100)
    height = int(image.shape[0] * scale_percent / 100)
    dim = (width, height)
    # resize image
   
    image = cv2.resize(cv2.cvtColor(image, cv2.COLOR_RGB2BGR), dim, interpolation = cv2.INTER_AREA)        
    cv2.imshow('Head Pose Estimation', image)
    
    if cv2.waitKey(5) & 0xFF == ord('q'):
      break
cv2.destroyAllWindows()
cap.release()
