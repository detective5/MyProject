using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using System.Configuration;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using FaceppSDK;
using System.Net;
namespace FaceLock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FaceService fs = new FaceService(ConfigurationManager.AppSettings["APIKey"].ToString(), ConfigurationManager.AppSettings["APISecret"].ToString());

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        //[DllImport("gdi32.dll")]
        //private static extern bool DeleteObject(IntPtr hObject);
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            if (Application.Current.Dispatcher == null)
                return null; // Is it possible?

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // You need to specify the image format to fill the stream. 
                    // I'm assuming it is PNG
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Make sure to create the bitmap in the UI thread
                    //if (InvokeRequired)
                    //    return (BitmapSource)Application.Current.Dispatcher.Invoke(
                    //        new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                    //        DispatcherPriority.Normal,
                    //        memoryStream);

                    return CreateBitmapSourceFromBitmap(memoryStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //private static bool InvokeRequired
        //{
        //    get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        //}

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }
        public MainWindow()
        {

            InitializeComponent();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo device in videoDevices)
            {
                deviceComboBox.Items.Add(device.Name);
            }

        }

        private void RecordCapture(object sender, RoutedEventArgs e)
        {
            RestoreDefault();
            if (deviceComboBox.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Select a device");
                return;
            }
            if (userName.Text == "")
            {
                System.Windows.MessageBox.Show("Please type in your name");
                return;
            }
            RunCamera("Default");
        }
        private void RunCamera(object sender, RoutedEventArgs e)
        {
            RunCamera("AddFaceStage");
        }
        private void RunCamera(string stage)
        {
            if (videoSource == null)
                videoSource = new VideoCaptureDevice();

            if (videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= videoSource_NewFrame;
                videoSource = null;
                PngBitmapEncoder pngE = new PngBitmapEncoder();
                pngE.Frames.Add(BitmapFrame.Create((BitmapSource)personImage.Source));
                using (Stream stream = File.Create(System.Environment.CurrentDirectory + "/temp.jpg"))
                {
                    pngE.Save(stream);
                }
                DetectResult detectResult = fs.Detection_DetectImg(System.Environment.CurrentDirectory + "/temp.jpg");
                File.Delete(System.Environment.CurrentDirectory + "/temp.jpg");
                if (stage == "Default")
                {
                    DefaultStage(detectResult);
                    camera.Content = "Run Camera";

                }

                else
                {
                    AddFaceStage(detectResult);
                    cameraTab.Content = "Run Camera";
                }
                //videoSource.Stop();         
            }
            else
            {
                videoSource = new VideoCaptureDevice(videoDevices[deviceComboBox.SelectedIndex].MonikerString);
                videoSource.NewFrame += videoSource_NewFrame;
                videoSource.Start();
                if (stage == "Default")
                    camera.Content = "Detect";
                else
                    cameraTab.Content = "Detect";
            }
        }
        private void AddFaceStage(DetectResult detectResult)
        {
            if (detectResult.face.Count != 1)
            {
                System.Windows.MessageBox.Show("No face detected, or more than one faces detected");
                return;
            }
            //perform add face to person
            ManageResult result = fs.Person_AddFaceByName(userName.Text, detectResult.face[0].face_id);
            if (result == null)
                return;
            if (result.success)
            {
                fs.Train_VerifyByName(userName.Text);
                System.Windows.MessageBox.Show("Face Added");
            }
        }
        private void DefaultStage(DetectResult detectResult)
        {
            if (detectResult.face.Count == 1)
            {
                VerifyResult person = fs.Recognition_VerifyByName(detectResult.face[0].face_id, userName.Text);
                status.Content = "Hello " + userName.Text + ", What would you like to do?";

                if (person == null)
                {
                    PersonBasicInfo info = fs.Person_Create(userName.Text, detectResult.face[0].face_id);
                    AsyncResult a = fs.Train_VerifyById(info.person_id);

                }
                else
                {
                    if (!person.is_same_person)
                        status.Content = "NO! You are not " + userName.Text;
                    else
                    {
                        lockFile.IsEnabled = true;
                        UnlockFile.IsEnabled = true;
                        Increase.IsEnabled = true;
                    }
                }
            }
            else
            {
                status.Content = "No face detected";
            }
        }
        private void RestoreDefault()
        {
            
            status.Content = "";
            defaultTab.IsSelected = true;
            lockFile.IsEnabled = false;
            UnlockFile.IsEnabled = false;
            Increase.IsEnabled = false;
        }
        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap a = eventArgs.Frame;



            ImageSource imgSource = CreateBitmapSourceFromBitmap(a);
            this.Dispatcher.Invoke((Action)(() =>
            {
                //System.Windows.Controls.Image temp = new System.Windows.Controls.Image();
                //temp.Source=imgSource;
                //faceCanvas.Children.Add(temp);

                personImage.Source = imgSource;

            }));

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null)
            {
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();

                    videoSource.NewFrame -= videoSource_NewFrame;
                    videoSource = null;
                }
            }
        }

        private void ChangeMenu(object sender, RoutedEventArgs e)
        {
            increaseAccuracyTab.IsSelected = true;
        }
        // Use the password to generate key bytes.
        private static void MakeKeyAndIV(string password, byte[] salt, int key_size_bits, int block_size_bits, out byte[] key, out byte[] iv)
        {
            Rfc2898DeriveBytes derive_bytes = new Rfc2898DeriveBytes(password, salt, 1000);

            key = derive_bytes.GetBytes(key_size_bits / 8);
            iv = derive_bytes.GetBytes(block_size_bits / 8);
        }

        // Encrypt the data in the input stream into the output stream.
        public static void CryptStream(string password, Stream in_stream, Stream out_stream, bool encrypt)
        {
            // Make an AES service provider.
            AesCryptoServiceProvider aes_provider = new AesCryptoServiceProvider();

            // Find a valid key size for this provider.
            int key_size_bits = 0;
            for (int i = 1024; i > 1; i--)
            {
                if (aes_provider.ValidKeySize(i))
                {
                    key_size_bits = i;
                    break;
                }
            }

            // Get the block size for this provider.
            int block_size_bits = aes_provider.BlockSize;

            // Generate the key and initialization vector.
            byte[] key = null;
            byte[] iv = null;
            byte[] salt = { 0x0, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0xF1, 0xF0, 0xEE, 0x21, 0x22, 0x45 };
            MakeKeyAndIV(password, salt, key_size_bits, block_size_bits, out key, out iv);

            // Make the encryptor or decryptor.
            ICryptoTransform crypto_transform;
            if (encrypt)
            {
                crypto_transform = aes_provider.CreateEncryptor(key, iv);
            }
            else
            {
                crypto_transform = aes_provider.CreateDecryptor(key, iv);
            }

            // Attach a crypto stream to the output stream.
            // Closing crypto_stream sometimes throws an
            // exception if the decryption didn't work
            // (e.g. if we use the wrong password).
            try
            {
                using (CryptoStream crypto_stream = new CryptoStream(out_stream, crypto_transform, CryptoStreamMode.Write))
                {
                    // Encrypt or decrypt the file.
                    const int block_size = 1024;
                    byte[] buffer = new byte[block_size];
                    int bytes_read;
                    while (true)
                    {
                        // Read some bytes.
                        bytes_read = in_stream.Read(buffer, 0, block_size);
                        if (bytes_read == 0) break;

                        // Write the bytes into the CryptoStream.
                        crypto_stream.Write(buffer, 0, bytes_read);
                    }
                } // using crypto_stream 
            }
            catch
            {
            }

            crypto_transform.Dispose();
        }
        private void WriteDownFileSize(string length, string dir, string fileName)
        {
            string file = dir + @"\" + fileName + "Size";
            using (StreamWriter outputFile = new StreamWriter(file, false))
            {
                outputFile.WriteLine(length);
            }
        }
        private string ReadSize(string file)
        {
            string size = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file))
                {
                    // Read the stream to a string, and write the string to the console.
                    size = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return size;
        }
        private void Encrypt(object sender, RoutedEventArgs e)
        {

            if (FileTextBox.Text == "")
                return;
            string inputFile = FileTextBox.Text;
            string dir = System.IO.Path.GetDirectoryName(inputFile);
            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(inputFile);
            string extension = System.IO.Path.GetExtension(inputFile);
            string outputFile = dir + @"\" + @"Encrypted_" + fileNameWithoutExt + extension;
            try
            {
                PersonInfo info = fs.Person_GetInfoByName(userName.Text);
                using (FileStream in_stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    WriteDownFileSize(in_stream.Length.ToString(), dir, fileNameWithoutExt);
                    using (FileStream out_stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        // Encrypt/decrypt the input stream into the output stream.
                        CryptStream(info.person_id, in_stream, out_stream, true);
                    }
                }
                //Delete the original file
                File.Delete(inputFile);
                MessageBox.Show("File Encrypted");
                FileTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ChooseFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                FileTextBox.Text = dlg.FileName;
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            if (FileTextBox.Text == "")
                return;
            string inputFile = FileTextBox.Text;
            string dir = System.IO.Path.GetDirectoryName(inputFile);
            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(inputFile);
            string extension = System.IO.Path.GetExtension(inputFile);
            if (fileNameWithoutExt.Substring(0, 10) == "Encrypted_")
            {
                fileNameWithoutExt = fileNameWithoutExt.Substring(10, fileNameWithoutExt.Length - 10);
            }
            string outputFile = dir + @"\" + fileNameWithoutExt + extension;
            try
            {
                PersonInfo info = fs.Person_GetInfoByName(userName.Text);
                using (FileStream in_stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream out_stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        // Encrypt/decrypt the input stream into the output stream.
                        CryptStream(info.person_id, in_stream, out_stream, false);

                    }
                }

                //Final Check
                FileInfo outputFileInfo = new FileInfo(outputFile);
                string originalSize = ReadSize(dir + @"\" + fileNameWithoutExt + "Size");
                string finalSize = outputFileInfo.Length.ToString();
                string deleteFile = outputFile;

                string message = "Wrong person!";

                if (originalSize == finalSize)
                {
                    deleteFile = inputFile;
                    message = "File Decrypted";
                }
                //result
                File.Delete(deleteFile);
                MessageBox.Show(message);
                FileTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }








    }
}
