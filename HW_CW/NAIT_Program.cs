using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace HW_CW
{
    class NAIT_Program
    {

        public void classificaition_example()
        {


            string image_dir = "../../example_data/cls_ex_img/";
            List<string> imagePaths = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                /*
                Load example images for example code
                */
                imagePaths.Add(image_dir + "normal" + i + ".png");
            }
            for (int i = 0; i < 11; i++)
            {
                /*
                Load example images for example code
                */
                imagePaths.Add(image_dir + "rotten" + i + ".png");
            }

            nrt.Status status;
            /* 
            Trained Model File Path
            */
            string model_path = "../../example_data/cls_ex_model.net";

            /*
            Initialize the Model object through the trained model file.
            
            If you want to get cam (class activation map) output, nrt::Model::MODELIO_OUT_CAM flag must be set as below.
            nrt.Model model = new nrt.Model(model_path, nrt.Model.MODELIO_OUT_CAM);

            The use of cam has an effect on the execution speed latency.
            The execution function may be slightly slower.
            */
            nrt.Model model = new nrt.Model(model_path, nrt.Model.MODELIO_OUT_CAM);


            if (model.get_status() != nrt.Status.STATUS_SUCCESS)
            {
                Console.WriteLine("Model initialization failed.  : " + nrt.nrt.get_last_error_msg());
                return;
            }
            int num_classes = model.get_num_classes();
            Console.WriteLine("num_classes " + num_classes);
            for (int i = 0; i < num_classes; i++)
            {
                Console.WriteLine("class " + i + " : " + model.get_class_name(i));
            }

            int num_inputs = model.get_num_inputs();

            nrt.Shape input_image_shape = model.get_input_shape(0);
            Console.Write(model.get_input_name(0) + " [");
            for (int j = 0; j < input_image_shape.num_dim; j++)
                Console.Write(input_image_shape.get_axis(j) + " ");
            Console.WriteLine("]");
            nrt.DType input_dtype = model.get_input_dtype(0);
            Console.WriteLine(" DType: " + nrt.nrt.dtype_to_str(input_dtype));

            nrt.InterpolationType resize_method = model.get_InterpolationType(0);

            /*
            Print the shape and type of the output
            */
            int num_outputs = model.get_num_outputs();
            int prob_idx = -1, pred_idx = -1, cam_idx = -1;
            for (int i = 0; i < num_outputs; i++)
            {
                /*
                The output of classification is MODELIO_OUT_PROB, which is a probability value for each class,
                and MODELIO_OUT_PRED, which is an index value of a predicted class.

                You can check the index of these outputs and retrieve the actual values through these indexes for real predictions
                */
                int output_flag = model.get_output_flag(i);
                if (output_flag == nrt.Model.MODELIO_OUT_PRED)
                {
                    pred_idx = i;
                    Console.WriteLine("pred_idx " + pred_idx);
                }
                else if (output_flag == nrt.Model.MODELIO_OUT_PROB)
                {
                    prob_idx = i;
                    Console.WriteLine("prob_idx " + prob_idx);
                }
                else if (output_flag == nrt.Model.MODELIO_OUT_CAM)
                {
                    /*
                    If you want to get cam (class activation map) output, when creating Model object, nrt.Model.MODELIO_OUT_CAM flag must be set as below.
                    nrt.Model model = new nrt.Model(model_path, nrt.Model.MODELIO_OUT_CAM);
                    */
                    cam_idx = i;
                    Console.WriteLine("cam_idx " + cam_idx);
                }
                nrt.Shape shp = model.get_output_shape(i);
                Console.Write("output " + i + " " + model.get_output_name(i) + " [");
                for (int j = 0; j < shp.num_dim; j++)
                    Console.Write(shp.get_axis(j) + " ");
                Console.WriteLine("] DType: " + nrt.nrt.dtype_to_str(model.get_output_dtype(i)));
            }

            int num_devices = nrt.Device.get_num_devices();
            Console.WriteLine("num_devices " + num_devices);


            nrt.Device dev = nrt.Device.get_device(0);

            int batch_size = 2;

            /*
            Executor creation for prediction
            Batch size is input for internal prediction
            When an input larger than the batch size comes in during prediction, the execution is automatically divided into batch sizes.

            It can also be run with a batch size  smaller than the batch_size value used to initialize the executor for the actual excution.
            However, it is recommended to initialize it to the same value as the batch_size that will be used for the actual excution.

            Creating model and executor objects can take up to tens of seconds.
            */
            nrt.Executor executor = new nrt.Executor(model, dev, batch_size);
            if (executor.get_status() != nrt.Status.STATUS_SUCCESS)
            {
                Console.WriteLine("Executor initialization failed. : " + nrt.nrt.get_last_error_msg());
                return;
            }

            nrt.NDBufferList outputs = new nrt.NDBufferList();
            nrt.NDBuffer output_prob;
            nrt.NDBuffer output_pred;
            nrt.NDBuffer thresholded_pred = new nrt.NDBuffer();
            nrt.NDBuffer output_cam;
            nrt.NDBuffer cam_colormap = new nrt.NDBuffer();

            int input_h = input_image_shape.get_axis(0);
            int input_w = input_image_shape.get_axis(1);
            int input_c = input_image_shape.get_axis(2);

            Console.WriteLine("input_h " + input_h);
            Console.WriteLine("input_w " + input_w);
            Console.WriteLine("input_c " + input_c);

            for (int i = 0; i < imagePaths.Count; i += batch_size)
            {
                int current_batch_size = Math.Min(batch_size, imagePaths.Count - i);

                string image_paths = "";
                for (int j = 0; j < current_batch_size; j++)
                {
                    image_paths += imagePaths[i + j] + "\n";
                }

                /*
                NRT's functions use the NDBuffer data type for input and output, and get_shape () and get_dtype () provide information about the size, shape, and type
                Create an NDBuffer from the image paths using the load_images method.
                Each path in image_paths must be separated by newlines.
                The load_images method resizes the image to the size of the input shape. And you can specify the shape and resize method.
                */
                nrt.NDBuffer images = nrt.NDBuffer.load_images(new nrt.Shape(input_h, input_w, input_c), image_paths, resize_method);

                // To upload the image data already loaded in memory to the NDBuffer, you can use the method of initializing the NDBuffer by specifying the Shape and Dtype and copying the data there.
                // In this case, the size of the image data must be consistent with the Shape and Dtype defined when creating the NDBuffer and must be continuous.
                // In this case, if the shape that created the NDBuffer and the input shape of the model are different, you can match them through the resize function, and you can select the resize method.
                // See the commented code below.
                // nrt.NDBuffer image_buff = new nrt.NDBuffer(new nrt.Shape(current_batch_size, input_h, input_w, input_c), input_dtype);
                // for (int j = 0; j < current_batch_size; j++)
                //     image_buff.copy_from_buffer_uint8(j , byte_buff, (ulong)byte_buff.Length); // Copy one image to each batch location
                // status = nrt.nrt.resize(images, resized_images, input_image_shape, resize_method);

                /*
                Perform prediction through executor.
                */
                status = executor.execute(images, outputs);
                if (status != nrt.Status.STATUS_SUCCESS)
                {
                    Console.WriteLine("prediction failed.  : " + nrt.nrt.get_last_error_msg());
                    return;
                }

                output_prob = outputs.get_at(prob_idx);
                nrt.Shape output_prob_shape = output_prob.get_shape();

                float[] prob_buff = new float[output_prob.get_total_size()];
                int prob_vector_length = output_prob_shape.get_axis(1);
                int actual_copy_size = output_prob.copy_to_buffer_float32(prob_buff, (ulong)prob_buff.Length);
                if (actual_copy_size != prob_buff.Length)
                {
                    Console.WriteLine("Copying is not complete. Please check the NDBuffer's shpae, dtype, and buffer size. ");
                    return;
                }
                /*
                Since the DType value of the output_prob is DTYPE_FLOAT32, you can get the float pointer from the NDBuffer and check the actual Probability value.
                */
                for (int j = 0; j < output_prob_shape.get_axis(0); j++)
                {
                    Console.WriteLine(i + j + "th image name : " + imagePaths[i + j]);
                    Console.Write("probability value for each class : [");
                    /*
                    The value of output_prob_shape.dims[1] is equal to num_classes. Thus, you can get the probability value for each class.
                    */
                    for (int cls_idx = 0; cls_idx < prob_vector_length; cls_idx++)
                    {
                        Console.Write(prob_buff[j * prob_vector_length + cls_idx] + ", ");

                    }
                    Console.WriteLine("]");
                }

                output_pred = outputs.get_at(pred_idx);
                int[] pred_buff = new int[output_pred.get_total_size()];

                actual_copy_size = output_pred.copy_to_buffer_int32(pred_buff, (ulong)pred_buff.Length);
                if (actual_copy_size != pred_buff.Length)
                {
                    Console.WriteLine("Copying is not complete. Please check the NDBuffer's shpae, dtype, and buffer size. ");
                    return;
                }
                /*
                Since the DType value of the output_prob is DTYPE_INT32, you can get the int pointer from the NDBuffer and check the actual class index value.
                */
                nrt.Shape output_pred_shape = output_pred.get_shape();
                for (int j = 0; j < output_pred_shape.get_axis(0); j++)
                {
                    Console.WriteLine(i + j + "th image - Prediction class index(Not thresholded): " + pred_buff[j]);

                }

                /*
                You can also apply thresholds to probability maps as shown below.
                If the class with the highest probability does not exceed Theshold, it is output as Unknown class.
                */
                float prob_threshold = 0.8f;
                status = nrt.nrt.prob_map_threshold(output_prob, prob_threshold, thresholded_pred);
                if (status != nrt.Status.STATUS_SUCCESS)
                {
                    Console.WriteLine("prob_map_threshold failed.  : " + nrt.nrt.get_last_error_msg());
                    return;
                }

                nrt.Shape thresholded_pred_shape = thresholded_pred.get_shape();
                int[] thresholded_pred_buff = new int[thresholded_pred.get_total_size()];

                actual_copy_size = thresholded_pred.copy_to_buffer_int32(thresholded_pred_buff, (ulong)thresholded_pred_buff.Length);
                if (actual_copy_size != thresholded_pred_buff.Length)
                {
                    Console.WriteLine("Copying is not complete. Please check the NDBuffer's shpae, dtype, and buffer size. ");
                    return;
                }

                for (int j = 0; j < thresholded_pred_shape.get_axis(0); j++)
                {
                    int cls = thresholded_pred_buff[j];
                    Console.WriteLine(i + j + "th image - Prediction class index(thresholded): " + cls + ((cls < 0) ? "(Unknown)" : ""));
                }
                if (cam_idx != -1)
                {
                    /*
                    If you want to get cam(class activation map) output, when creating Model object, nrt.Model.MODELIO_OUT_CAM flag must be set as below.
                    nrt.Model model = new nrt.Model(model_path, nrt.Model.MODELIO_OUT_CAM);
                    */
                    output_cam = outputs.get_at(cam_idx);

                    /*
                    In nrt, cam output is basically output as a grayscale image with each pixel value between 0 and 255.

                    Use the convert_to_colormap function to get a color map as shown on the UI screen.
                    */
                    nrt.Shape output_cam_shape = output_cam.get_shape();


                    status = nrt.nrt.convert_to_colormap(output_cam, cam_colormap);

                    if (status != nrt.Status.STATUS_SUCCESS)
                    {
                        Console.WriteLine("convert_to_colormap failed.  : " + nrt.nrt.get_last_error_msg());
                        return;
                    }

                    //You can copy the contents of the NDBuffer into memory using the copy_to_buffer_uint8 function.
                    //When executed in batch units greater than 1, cam_colormap has multiple images.
                    //So you can copy multiple images at once, or you can copy individual images by entering the first dimension index in the copy_to_buffer_uint8 function.
                    //cam_colormap.copy_to_buffer_uint8(buff, (ulong)buff.Length); 
                    //cam_colormap.copy_to_buffer_uint8(j, buff, (ulong)buff.Length);

                }
            }
        }



        //static void Main(string[] args)
        //{
        //    classificaition_example();
        //}
    }

}
