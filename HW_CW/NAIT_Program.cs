using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HW_CW
{
    class NAIT_Program
    {
        //public static void segmentation_example()
        //{

        //}

        public string test1(string str)
        {
            str = str + "NYAAAA";

            return str;
        
        }
        

        
        public List<string> imagePaths()
        {
            List<string> imagePathsList = new List<string>();

            string image_dir = "./example_data/img2/";
            for (int i = 0; i < 8; i++)
            {
                
                //Load example images for example code
                
                imagePathsList.Add(image_dir + i + ".png");
            }

            return imagePathsList;
        
        }

        
        public void model_read(List<string> imagePaths)
        { 
            nrt.Status status;
            
            //Trained Model File Path
            
            string model_path = "../../example_data/seg_ex_model.net";

            
            //Initialize the Model object through the trained model file.
            
            nrt.Model model = new nrt.Model(model_path);
            if (model.get_status() != nrt.Status.STATUS_SUCCESS)
            {
                Console.WriteLine("Model initialization failed.  : " + nrt.nrt.get_last_error_msg());
                return;
            }

           
            //Print the number of classes and class names assigned to each class index
            
            int num_classes = model.get_num_classes();
            Console.WriteLine("num_classes " + num_classes);
            for (int i = 0; i < num_classes; i++)
            {
                Console.WriteLine("class " + i + " : " + model.get_class_name(i));
            }

            
            //Print the shape and type of the input

            //Segmentation takes one image as input. (num_inputs==1)
            //The size and type of the input image can be checked with model.get_input_shape(0) and model.get_input_dtype (0).

            //The size of the image that can be checked with get_input_shape(0) is [patch_height, patch_width, 3].
            //and when using batch exection, [batch_size, patch_height, patch_width, 3] can be used as the input size.
            
            int num_inputs = model.get_num_inputs();

            nrt.Shape input_image_shape = model.get_input_shape(0);
            Console.Write(model.get_input_name(0) + " [");
            for (int j = 0; j < input_image_shape.num_dim; j++)
                Console.Write(input_image_shape.get_axis(j) + " ");
            Console.Write("]");
            nrt.DType input_dtype = model.get_input_dtype(0);
            Console.WriteLine(" DType: " + nrt.nrt.dtype_to_str(input_dtype));

            nrt.InterpolationType resize_method = model.get_InterpolationType(0);



            bool patch_mode = model.is_patch_mode(0);

            float scale_factor = model.get_scale_factor();
            if (patch_mode)
            {
                Console.WriteLine("scale_factor " + scale_factor);
            }

            
            //Print the shape and type of the output
            //Output of the segmentation is a prediction map where shape is equal to the height and width of the input, and the value of each pixel represents the class index.
            

            int num_outputs = model.get_num_outputs();
            for (int i = 0; i < num_outputs; i++)
            {
                nrt.Shape shp = model.get_output_shape(i);
                Console.Write("output " + i + " " + model.get_output_name(i) + " [");
                for (int j = 0; j < shp.num_dim; j++)
                    Console.Write(shp.get_axis(j) + " ");
                Console.Write("] DType: " + nrt.nrt.dtype_to_str(model.get_output_dtype(i)));
            }

            int num_devices = nrt.Device.get_num_devices();
            Console.WriteLine("num_devices " + num_devices);
            
            //Get GPU0 device
            
            nrt.Device dev = nrt.Device.get_device(0);

            int batch_size = 1;

            
            //Executor creation for prediction
            //Batch size is input for internal prediction
            //When an input larger than the batch size comes in during prediction, the execution is automatically divided into batch sizes.

            //It can also be run with a batch size  smaller than the batch_size value used to initialize the executor for the actual excution.
            //However, it is recommended to initialize it to the same value as the batch_size that will be used for the actual excution.

            //Creating model and executor objects can take up to tens of seconds.
            

            nrt.Executor executor = new nrt.Executor(model, dev, batch_size);
            if (executor.get_status() != nrt.Status.STATUS_SUCCESS)
            {
                Console.WriteLine("Executor initialization failed. : " + nrt.nrt.get_last_error_msg());
                return;
            }

            
            //If the original image is larger than the patch size, it cannot be used as an input immediately, but should be used as an input after patching as
            
            int input_h = 512;
            int input_w = 512;
           
            //In the current version, all input images are processed as 3 channels. Grayscale images must be converted to 3 channels.
            
            int input_c = 3;
            int input_image_byte_size = input_h * input_w * input_c;
        
            nrt.NDBuffer resized_image_buff = new nrt.NDBuffer();
            nrt.NDBufferList outputs = new nrt.NDBufferList();
            nrt.NDBuffer image_patch_buff = new nrt.NDBuffer();
            nrt.NDBuffer patch_info = new nrt.NDBuffer();
            nrt.NDBuffer merged_output = new nrt.NDBuffer();
            
            //Set the size threshold.
            //height_thres  : Height threshold
            //width_thres : Width threshold
            //thres_cond
            //    Case thres_cond == 0, it changes to background when the size of the detected area satisfies both height_thres and width_thres. (AND condition)
            //    Case thres_cond == 1, it changes to the background area when the size of the detected area satisfies either height_thres or width_thres. (OR condition)
            
            int height_thres = 16;
            int width_thres = 16;
            int thres_cond = 0;
            nrt.NDBuffer size_threshold_buf = nrt.NDBuffer.make_size_thres(height_thres, width_thres, thres_cond);
            nrt.NDBuffer bounding_rects = new nrt.NDBuffer();
            
            for (int i = 0; i < imagePaths.Count; i += batch_size)
            {
                int current_batch_size = Math.Min(batch_size, imagePaths.Count - i);

                string image_paths = "";
                for (int j = 0; j < current_batch_size; j++)
                {
                    image_paths += imagePaths[i + j] + "\n";
                }

                
                //NRT's functions use the NDBuffer data type for input and output, and get_shape () and get_dtype () provide information about the size, shape, and type
                //Create an NDBuffer from the image paths using the load_images method.
                //Each path in image_paths must be separated by newlines.
                //The load_images method resizes the image to the size of the input shape. And you can specify the shape and resize method.
                
                nrt.NDBuffer images = nrt.NDBuffer.load_images(new nrt.Shape(input_h, input_w, input_c), image_paths, resize_method);

                // To upload the image data already loaded in memory to the NDBuffer, you can use the method of initializing the NDBuffer by specifying the Shape and Dtype and copying the data there.
                // In this case, the size of the image data must be consistent with the Shape and Dtype defined when creating the NDBuffer and must be continuous.
                // See the commented code below.

                // nrt.NDBuffer image_buff = new nrt.NDBuffer(new nrt.Shape(current_batch_size, image_height, image_width, image_channel), input_dtype);
                // for (int j = 0; j < current_batch_size; j++)
                //     image_buff.copy_from_buffer_uint8(j , byte_buff, (ulong)byte_buff.Length); // Copy one image to each batch location
                // 


                if (patch_mode)
                {
                    Console.WriteLine("patch_mode execution.");
                    
                    //If the patch mode value is true, the model training is done in patch units, so it must be divided into appropriate patches before entering the model.\n
                    //In patch mode, the original image is trained after resizing according to the scale factor value.\n
                    //After checking scale_factor with get_scale_factor(), you must use the resize function that receives scale_factor as input or resize it to the following size.\n
                    //resize_height = int(original_height * scale_factor)\n
                    //resize_width = int(original_width * scale_factor)\n

                    //So if the scale_factor wasn't 1, you would need to resize it with the following function
                    //and the final output prediction map would also change to the size multiplied by the scale_factor.
                    
                    status = nrt.nrt.resize(images, resized_image_buff, scale_factor, resize_method);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("resize failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    
                    //Split the image into patches.
                    //The first dimension value of the image_patch_buff output is increased by the total number of patches generated for each input image,
                    //and the patched information is returned to patch_info to be used in subsequent merge operations

                    //The patch size should be the same as the size used for Model training and can be checked through the Model object.
                    //So you have to split it into patches via the function below to use it for execution.
                    
                    status = nrt.nrt.extract_patches_to_target_shape(resized_image_buff, input_image_shape, image_patch_buff, patch_info);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("extract_patches_to_target_shape failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    
                    //Perform prediction through executor.
                    //If the size of the image used for training is the same as the patch size and the scale_factor is set to the default value of 1,
                    //you can skip the resize and extract_patches_to_target_shape steps shown above and execute the execute function to get the output prediction map.
                    
                    status = executor.execute(image_patch_buff, outputs);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("prediction failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    
                    //Merges the prediction results into the shape corresponding to the original image.

                    //In the case of the segmentation model,
                    //the prediction map is output only, so the num_outputs value is 1,
                    //so you can get the prediction map from outputs with outputs.get_at(0)
                    
                    status = nrt.nrt.merge_patches_to_orginal_shape(outputs.get_at(0), patch_info, merged_output);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("merge_patches_to_orginal_shape failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                }
                else
                {
                    Console.WriteLine("non patch_mode execution.");
                    
                    //If it is not in the patch mode, if the original image and the model input size are different, it must be resized according to the model input size
                    //This input size can be checked through the get_input_shape method on the model object.

                    //If the original image size and the model input size are the same, you can skip the resize procedure.
                    
                    status = nrt.nrt.resize(images, resized_image_buff, input_image_shape, resize_method);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("resize failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    
                    //Perform prediction through executor.
                    
                    status = executor.execute(resized_image_buff, outputs);
                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("prediction failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    merged_output = outputs.get_at(0);
                }


                
                //Information about the size and type of merged_output that is the result of the final output prediction map can be obtained with get_shape() and get_dtype().
                
                nrt.Shape merged_output_shape = merged_output.get_shape();
                nrt.DType merged_output_dtype = merged_output.get_dtype();

                
                //Threshold the predictions by height and width.
                
                status = nrt.nrt.pred_map_threshold_by_size(merged_output, bounding_rects, size_threshold_buf, num_classes);
                if (status != nrt.Status.STATUS_SUCCESS)
                {
                    Console.WriteLine("pred_map_threshold_by_size failed.  : " + nrt.nrt.get_last_error_msg());
                    return;
                }
                nrt.Shape bounding_rects_shape = bounding_rects.get_shape();

                int[] bounding_rects_buff = new int[bounding_rects.get_total_size()];

                int actual_copy_size = bounding_rects.copy_to_buffer_int32(bounding_rects_buff, (ulong)bounding_rects_buff.Length);
                if (actual_copy_size != bounding_rects_buff.Length)
                {
                    Console.WriteLine("Copying is not complete. Please check the NDBuffer's shpae, dtype, and buffer size. ");
                    return;
                }

                
                //The shape of bounding_rects is as follows : [n, 6]
                //n : Total number of predicted areas
                //The six values in the second dimension are :
                //  0 : batch index (if four-dimensional batch image shape [n, height, width, channel] ) for the input image_buff
                //  1 : Upper left x coordinate of detection area rectangle
                //  2 : Upper left y coordinate of detection area rectangle
                //  3 : Height of detection area rectangle
                //  4 : Width of detection area rectangle
                //  5 : Class index of the detection area rectangle
               
                for (int j = 0; j < bounding_rects_shape.get_axis(0); j++)
                {

                    int image_batch_index = bounding_rects_buff[6 * j + 0];
                    int rect_x = bounding_rects_buff[6 * j + 1];
                    int rect_y = bounding_rects_buff[6 * j + 2];
                    int rect_h = bounding_rects_buff[6 * j + 3];
                    int rect_w = bounding_rects_buff[6 * j + 4];
                    int rect_class_index = bounding_rects_buff[6 * j + 5];

                    Console.WriteLine(" x : " + rect_x + " y : " + rect_y + " height : " + rect_h + " width : " + rect_w + " class idx : " + rect_class_index);

                }

                
                //Since the DType value of the prediction map is DTYPE_UINT8, you can get the byte data from the NDBuffer and check the actual image value.
                //The background area is 0. A value with class index of 0 means background    
                
                byte[] output_buff = new byte[merged_output.get_total_size()];

                actual_copy_size = merged_output.copy_to_buffer_uint8(output_buff, (ulong)output_buff.Length);
                if (actual_copy_size != output_buff.Length)
                {
                    Console.WriteLine("Copying is not complete. Please check the NDBuffer's shpae, dtype, and buffer size. ");
                    return;
                }

            }




    
        }
        /*
        static void Main(string[] args)
        {
            segmentation_example();
        }
        */
    }
}
