import random
import copy
import png
import numpy
import os

def cls():
    os.system('cls' if os.name=='nt' else 'clear')



# Generates a single Random Dot image
def Generate_RDS(image_size):
    
    # Generate Empty 2D Array
    image = [[0 for x in range(image_size)] for y in range(image_size)]             

    # Assign whether black or white randomly
    for y in range(image_size):
        for x in range(image_size):
            image[y][x] = random.randint(0,1)

    return image
    

# Shifts the pixels of a given image, returning the duplicated image array
def Shift_Pixels(image, image_size, shift_size, shift_amout, shift_percentage):
    
    image_temp = Duplicate_Image(image, image_size)
    
    start_x = int((image_size / 2) - (shift_size / 2))
    start_y = start_x



    if (shift_amout > 0):
        for y in range(start_y, start_y + shift_size, 1):
            for x in range(start_x, start_x + shift_size, 1):
                if (random.randint(1,100) <= shift_percentage):
                    image_temp[y][x] = image[y][x + shift_amout]
                    #image_temp[y][x + shift_amout] = random.randint(0,1) 
    else:
        for y in range(start_y + shift_size, start_y, -1):
            for x in range(start_x + shift_size, start_x, -1):
                if (random.randint(1,100) <= shift_percentage):
                    image_temp[y][x + shift_amout] = image[y][x]




    return image_temp

# Deep copying image array to avoid Python array referencing
def Duplicate_Image(image, image_size):
    clone_image = [[0 for x in range(image_size)] for y in range(image_size)]  
    for x in range(image_size):
        for y in range(image_size):
            clone_image[y][x] = image[y][x]

    return clone_image


# Converts the each row of the image array to a string so it can
# be used by the png.writer function from 'import png'
def Convert_Image_To_String(image, image_size):
    s = ['0' for y in range(image_size)]

    for y in range(image_size):
        row = ''
        for x in range(image_size):
            row += str(image[y][x])
        s[y] = row

    return s

def Scale_Image(image, image_size, scale):
     image_size_new = 4 * image_size
     image_new = [[0 for x in range(image_size_new)] for y in range(image_size_new)]  
     for y in range(image_size):
        for x in range(image_size):
            for j in range(scale):
                for i in range(scale):
                    image_new[y*scale+j][x*scale+i] = image[y][x]

     return image_new

# Saving Image
def Save_Image(image, name):
    size = len(image);
    image = Scale_Image(image, size, 4)
    size = size * 4

    s = Convert_Image_To_String(image, size)
    s = [[int(c) for c in row] for row in s]
    w = png.Writer(len(s[0]), len(s), greyscale=True, bitdepth=1)
    f = open(name, 'wb')
    w.write(f, s)
    f.close()

# Image Variables
image_size = 100    
shift_size = 60
shift_amount = 4

#VR: 300, 60, 4

#Image Quanitity
RDS_count = 1
RDS_location = 'RDS/'
RDS_name = 'RDS'

increment = 2
for i in range(RDS_count):
    for j in range(0, 101, increment):
        shift_percentage = j
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
        
        print((int)(((i*RDS_count)+j) / ((101)*RDS_count) * 100))



