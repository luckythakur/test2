using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net;

namespace ConsoleApplication28082010
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string fileName;
            string destFile;
            string sourcePath = ConfigurationManager.AppSettings["SourcePath"];
            string destinationPath = ConfigurationManager.AppSettings["DestinationPath"]; ;

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            if (Directory.Exists(sourcePath))
            {
                string[] files = Directory.GetFiles(sourcePath);

                foreach (string filepath in files)
                {
                    fileName = Path.GetFileName(filepath);
                    destFile = Path.Combine(destinationPath, fileName);
                    Console.WriteLine(fileName);
                    File.Copy(filepath, destFile, true);
                    File.Delete(filepath);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
       
            Console.ReadLine();
            */

            /*
             * File Download,Write Local Copy, Delete from FTP Location
             */
            string localPath = @"G:\FTPTrialLocalPath\";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://localhost/Source");
            request.Credentials = new NetworkCredential("khanrahim", "arkhan22");
            request.Method = WebRequestMethods.Ftp.ListDirectory;
                        
            StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
            request = null;

            string fileName = streamReader.ReadLine();

            while (fileName != null)
            {
                FtpWebRequest requestFileDelete = null;
                FtpWebResponse responseFileDelete = null;

                FtpWebRequest requestFileDownload = null;
                FtpWebResponse responseFileDownload = null;
                
                try
                {
                    Console.WriteLine(fileName);

                    requestFileDownload = (FtpWebRequest)WebRequest.Create("ftp://localhost/Source/" + fileName);
                    requestFileDownload.Credentials = new NetworkCredential("khanrahim", "arkhan22");
                    requestFileDownload.Method = WebRequestMethods.Ftp.DownloadFile;
                    responseFileDownload = (FtpWebResponse)requestFileDownload.GetResponse();

                    Stream responseStream = responseFileDownload.GetResponseStream();
                    FileStream writeStream = new FileStream(localPath + fileName, FileMode.Create);                
                
                    int Length = 2048;
                    Byte[] buffer = new Byte[Length];
                    int bytesRead = responseStream.Read(buffer, 0, Length);               
                    while (bytesRead > 0)
                    {
                        writeStream.Write(buffer, 0, bytesRead);
                        bytesRead = responseStream.Read(buffer, 0, Length);
                    }   
             
                    writeStream.Close();

                    Console.WriteLine("Download Done");
                    
                    requestFileDelete = (FtpWebRequest)WebRequest.Create("ftp://localhost/Source/" + fileName);
                    requestFileDelete.Credentials = new NetworkCredential("khanrahim", "arkhan22");
                    requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;
                    responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
                    Console.WriteLine("Delete status: {0}", responseFileDelete.StatusDescription);
                }
                catch (Exception exceptionObj)
                {
                    Console.WriteLine(exceptionObj.Message.ToString());
                }
                finally
                {
                    responseFileDelete = null;
                    requestFileDelete = null;

                    requestFileDownload = null;
                    responseFileDownload = null;
                }

                fileName = streamReader.ReadLine();
            }
            
            request = null;
            streamReader = null;

            /*
             * File Upload to a specific FTP Location 
             */
            FtpWebRequest requestFTPUploader = null;

            string[] files = Directory.GetFiles(localPath);

            foreach (string filepath in files)
            {
                fileName = Path.GetFileName(filepath);

                requestFTPUploader = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1/Destination/" + fileName);
                requestFTPUploader.Credentials = new NetworkCredential("khanrahim", "arkhan22");
                requestFTPUploader.Method = WebRequestMethods.Ftp.UploadFile;

                FileInfo fileInfo = new FileInfo(filepath);
                FileStream fileStream = fileInfo.OpenRead();

                int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];

                try
                {
                    Stream uploadStream = requestFTPUploader.GetRequestStream();
                    int contentLength = fileStream.Read(buffer, 0, bufferLength);

                    while (contentLength != 0)
                    {
                        uploadStream.Write(buffer, 0, contentLength);
                        contentLength = fileStream.Read(buffer, 0, bufferLength);
                    }
                    
                    uploadStream.Close();
                    fileStream.Close();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString());
                }

                Console.WriteLine(fileName + "Uploaded");
            }

            requestFTPUploader = null;
            Console.ReadLine();
        }
    }
}
