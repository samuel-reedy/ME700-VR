import random
import copy
import numpy as np
import imageio
import time


# Generates a single Random Dot image
def Generate_RDS(image_size):
    image = np.random.randint(2, size=(image_size, image_size))
    return image.tolist()
    

# Shifts the pixels of a given image, returning the duplicated image array
def Shift_Pixels(image, image_size, shift_size, shift_amout, shift_percentage):
    image_temp = np.copy(image)
    
    start_x = int((image_size / 2) - (shift_size / 2))
    start_y = start_x

    if (shift_amout > 0):
        for y in range(start_y, start_y + shift_size, 1):
            for x in range(start_x, start_x + shift_size, 1):
                if (random.randint(1,100) <= shift_percentage):
                    image_temp[y][x] = image[y][x + shift_amout]


    else:
        for y in range(start_y + shift_size, start_y, -1):
            for x in range(start_x + shift_size, start_x, -1):
                if (random.randint(1,100) <= shift_percentage):
                    image_temp[y][x + shift_amout] = image[y][x]


    return image_temp

# Saving Image
def Save_Image(image, name):
    image = np.multiply(image, 255)
    image = np.repeat(np.repeat(image, scale, axis=0), scale, axis = 1)

    imageio.imwrite(name, image.astype('uint8'))
    

# Image Variables
image_size = 100    
shift_size = 60
shift_amount = 4
scale = 4

#VR: 300, 60, 4

#Image Quanitity
RDS_count = 3*10
RDS_location = 'RDSZero/'
#RDS_location = 'RDS/'
RDS_name = 'RDS'

#percentages = [10,20,30,40,50,60,70,80,90,100]
percentages = [0]


increment = 10

for i in range(RDS_count):
    for j in range(len(percentages)):
        shift_percentage = percentages[j]
        # Right Shift 
        image = Generate_RDS(image_size)
        image_clone = Shift_Pixels(image, image_size, shift_size, shift_amount, shift_percentage)

        location_name = RDS_location + str(i+1) + '_' + str(shift_percentage) + '_' + 'R' 
        name = location_name + '_' + 'A' +'.png'
        Save_Image(image, name)
        name = location_name + '_' + 'B' +'.png'
        Save_Image(image_clone, name)

        # Left Shift
        image = Generate_RDS(image_size)
        image_clone = Shift_Pixels(image, image_size, shift_size, -shift_amount, shift_percentage)

        location_name = RDS_location + str(i+1) + '_' + str(shift_percentage) + '_' + 'L' 
        name = location_name + '_' + 'A' +'.png'
        Save_Image(image_clone, name)
        name = location_name + '_' + 'B' +'.png'
        Save_Image(image, name)

    print((int)(i / RDS_count * 100.0))






