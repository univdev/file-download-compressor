using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    class FTP
    {
        string host = null;
        string location = null;
        string username = null;
        string password = null;

        public FTP (string host, string location, string username, string password)
        {
            this.host = host;
            this.location = location;
            this.username = username;
            this.password = password;
        }
        public void DirectoryDownload (string root, string DirectoryName, string destination)
        {
            // Get the object used to communicate with the server.
            Uri uri = new Uri(host + "/" + root + "/" + DirectoryName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string resource = null;
            while ((resource = reader.ReadLine()) != null)
            {
                string directory = root + "/" + DirectoryName;
                string fileName = Path.GetFileName(resource).Substring(3);
                string downloadDirectory = destination + "/" + DirectoryName;
                string extension = Path.GetExtension(resource);
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                Console.WriteLine("extension: " + extension);
                Console.WriteLine("filename: " + fileName);
                if (extension == String.Empty)
                    this.DirectoryDownload(directory, fileName, downloadDirectory);
                else
                    this.FileDownload(directory, fileName, downloadDirectory);
            }

            reader.Close();
            response.Close();
        }
        public void FileDownload (string directory, string filename, string downloadDirectory)
        {
            try
            {
                Uri uri = new Uri(this.host + "/" + directory + "/" + filename);

                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(this.username, this.password);
                    request.DownloadFile(uri, downloadDirectory + "/" + filename);
                }
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void Compress(string path, string directoryName)
        {
            // Get the object used to communicate with the server.
            string folderPath = path + "/" + directoryName;
            string zipPath = path + "/" + directoryName + ".zip";
            ZipFile.CreateFromDirectory(folderPath, zipPath);
        }
        public void DeleteDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(path, false);
        }
        public void exec(string path, string date, string destination)
        {
            this.DirectoryDownload(path, date, destination);
            this.Compress(destination, date);
            this.DeleteDirectory(destination + "/" + date);
            MessageBox.Show("파일 다운로드가 완료되었습니다.");
        }
    }
}
