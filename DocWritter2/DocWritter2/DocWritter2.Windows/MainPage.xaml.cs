using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DocWritter2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        XmlSerializer serializer = new XmlSerializer(typeof(StudentInfo));
        List<StudentInfo> studentList = new List<StudentInfo>();
        List<CourseInfo> classList = new List<CourseInfo>();
        StudentInfo selectedStudent;
        CourseInfo selectedClass;
        public MainPage()
        {
            this.InitializeComponent();
            init();
            RefreshButton_Click(null, null);
            //
            /*StudentInfo aS = new StudentInfo();
            CourseInfo info = new CourseInfo();
            aS.StudentName = "ssss0";
            info.addSummary("FFFFFFFFFFFFFFFFFFFF");
            aS.ClassList.Add(info);
            aS.ClassList.Add(info);
            XmlSerializer serializer = new XmlSerializer(typeof(StudentInfo));
            MemoryStream ass = new MemoryStream();
            serializer.Serialize(ass, aS);
            string result = System.Text.Encoding.UTF8.GetString(ass.ToArray(), 0, ass.ToArray().Length);
            //textBlock1.Text = (String)a.ElementAt(0).Header;
            //a.ElementAt(0).DataContext = info;
            WriteDataToFileAsync("g21.s", result);
            var folder = ApplicationData.Current.LocalFolder;
            //textBlock2.Text = folder.Path;
            */
        }
        public void init()
        {
            _base.SizeChanged += _base_SizeChanged;
            ClassList.SelectionChanged += ClassList_SelectionChanged;
            StudentList.SelectionChanged += StudentList_SelectionChanged;
            AddButton.Click += AddButton_Click;
            removeButton.Click += RemoveButton_Click;
            refreshButton.Click += RefreshButton_Click;
            addText.Click += AddText_Click;
            cleanText.Click += CleanText_Click;
            cleanTopic.Click += CleanTopic_Click;
            addTopic.Click += AddTopic_Click;
            addClass.Click += AddClass_Click;
            cleanDate.Click += CleanDate_Click;
            topicsList.SelectionChanged += TopicsList_SelectionChanged;
            topicBox.GotFocus += TopicBox_GotFocus;
        }

        private void TopicBox_GotFocus(object sender, RoutedEventArgs e)
        {
            topicsList.SelectedItem = null;
            mainTextBox.Text = "";
        }

        private void TopicsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem item = (ComboBoxItem)e.AddedItems.First();
                String name = (String)item.Content;
                int place = selectedClass.Topics.IndexOf(name);
                if (place>-1)
                    mainTextBox.Text = selectedClass.Texts.ElementAt(place);
            }
            
        }
        public int check()
        {
            if (selectedStudent == null) return 1;
            if (selectedClass == null) return 2;
            return 3;
        }
        private void CleanDate_Click(object sender, RoutedEventArgs e)//
        {
            selectedStudent.ClassList.Remove(selectedClass);
            saveFile();
            ClassList.Items.Clear();
            loadClassList();
        }
        private async void AddClass_Click(object sender, RoutedEventArgs e)
        {
            if (check()==1) return;
            selectedClass = new CourseInfo();
            selectedClass.Date = datePicker.Date.Date;
            selectedClass.Start = startTime.Time;
            selectedClass.End = endTime.Time;
            if (selectedStudent.ClassList.Count > 0)
                selectedClass.SessionNumber = selectedStudent.ClassList.Last().SessionNumber + 1;
            else
                selectedClass.SessionNumber = 1;
            selectedStudent.ClassList.Add(selectedClass);
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, selectedStudent);
            string result = System.Text.Encoding.UTF8.GetString(stream.ToArray(), 0, stream.ToArray().Length);
            await WriteDataToFileAsync(selectedStudent.StudentName + ".S", result);
            ListViewItem selectedItem = null ;
            foreach (ListViewItem item in StudentList.Items)
            {
                String text=(String)item.Content;
                if (text.Equals(selectedStudent.StudentName))
                {
                    selectedItem = item;
                    break;
                }
            }
            ClassList.Items.Clear();
            loadClassList();
        }
        private async void AddTopic_Click(object sender, RoutedEventArgs e)
        {
            if (check() == 1 || check() == 2) return;
            selectedClass.addTopic(topicBox.Text, mainTextBox.Text);
            saveFile();
            topicBox.Text = "";
            mainTextBox.Text = "";
            List<string> topics = selectedClass.Topics;
            topicsList.Items.Clear();
            foreach (string topic in topics)
            {
                ComboBoxItem topicDispaly = new ComboBoxItem();
                topicDispaly.Content = topic;
                topicsList.Items.Add(topicDispaly);
            }
        }     
        private void CleanTopic_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem topic = (ComboBoxItem)topicsList.SelectedItem;
            if (topic == null) return;
            String Topic = (String)topic.Content;
            int location = selectedClass.Topics.IndexOf(Topic);
            selectedClass.Topics.Remove(Topic);
            selectedClass.Texts.RemoveAt(location);
            saveFile();
            topicBox.Text = "";
            mainTextBox.Text = "";
            List<string> topics = selectedClass.Topics;
            topicsList.Items.Clear();
            foreach (string topicOnList in topics)
            {
                ComboBoxItem topicDispaly = new ComboBoxItem();
                topicDispaly.Content = topicOnList;
                topicsList.Items.Add(topicDispaly);
            }
        }
        private void CleanText_Click(object sender, RoutedEventArgs e)//
        {
            mainTextBox.Text = "";
        }
        private async void AddText_Click(object sender, RoutedEventArgs e)//
        {
            if (check() == 1 || check() == 2) return;
            ComboBoxItem item = (ComboBoxItem)topicsList.SelectedItem;
            if (topicBox.Text.Equals(""))
            {
                String name = (String)item.Content;
                int place = selectedClass.Topics.IndexOf(name);
                selectedClass.changeTopic(name, mainTextBox.Text);
            }
            else
            {
                selectedClass.addTopic(topicBox.Text, mainTextBox.Text);
                selectedClass.changeTopic(topicBox.Text, mainTextBox.Text);
            }
            saveFile();
            topicBox.Text = "";
           // mainTextBox.Text = "";
            List<string> topics = selectedClass.Topics;
            topicsList.Items.Clear();
            foreach (string topic in topics)
            {
                ComboBoxItem topicDispaly = new ComboBoxItem();
                topicDispaly.Content = topic;
                topicsList.Items.Add(topicDispaly);
            }
        }
        async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
           
            studentList.Clear();
            StudentList.Items.Clear();
            ClassList.Items.Clear();
            classList.Clear();
            loadStudentList();
            /*
             *  StudentInfo[] studentLists= await this.ReadFileAllfile();
             *  foreach (var student in studentLists)
            {
                studentList.Add(student);
            }
            ListViewItem item = new ListViewItem();
            textBlock2.Text = "" + studentList.Count;
            foreach (StudentInfo student in studentList)
            {
                item = new ListViewItem();
                item.Content = student.StudentName;
                item.FontSize=20;
                StudentList.Items.Add(item);
            }
             MemoryStream stram=await ReadFileContentsAsync("g21.s");
             string result = System.Text.Encoding.UTF8.GetString(stram.ToArray(), 0, stram.ToArray().Length);
             mainTextBox.Text += result;
             textBlock2.Text= result;
             ListViewItem item = new ListViewItem();
             StudentInfo newStudent;
             using (var stream = new StringReader(result))
             {
                newStudent = (StudentInfo)serializer.Deserialize(stram);
             }
             item.Content = newStudent.StudentName;
             item.FontSize = 30;
             StudentList.Items.Add(item);*/
        }
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                await DeleteAsync(selectedStudent.StudentName + ".S");
                RefreshButton_Click(null, null);
            }
            catch (Exception)
            {
               
            }
            
        }
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StudentInfo newStudent = new StudentInfo();
            newStudent.StudentName = studentName.Text;
            ComboBoxItem gen = (ComboBoxItem)Gen.SelectedItem;
            if (gen == null) return;
            newStudent.Gen = (String)gen.Content;
            newStudent.Birth = birthday.Date.Date;
            newStudent.ClassType = classType.Text;
            studentName.Text = "Student Name";
            classType.Text = "Class Type";
            studentList.Add(newStudent);
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, newStudent);
            string result = System.Text.Encoding.UTF8.GetString(stream.ToArray(), 0, stream.ToArray().Length);
            await WriteDataToFileAsync(newStudent.StudentName + ".S", result);
            RefreshButton_Click(null,null);
        }
        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)e.AddedItems.First();
                String name = (String)item.Content;
                foreach (var student in studentList)
                {
                    if (student.StudentName.Equals(name))
                    {
                        selectedStudent = student;
                        break;
                    }
                }
                ClassList.Items.Clear();
                classList = selectedStudent.ClassList;
                foreach (CourseInfo course in classList)
                {
                    ListViewItem courseDisplay = new ListViewItem();
                    courseDisplay.Content = "Session " + course.SessionNumber;
                    ClassList.Items.Add(courseDisplay);
                }
               // studentList.in
            }
            
        }
        private void ClassList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)e.AddedItems.First();
                String name = (String)item.Content;
                foreach (var course in classList)
                {
                    String content = "Session " + course.SessionNumber;
                    if (content.Equals(name))
                    {
                        selectedClass = course;
                        break;
                    }
                }
                List<string> topics = selectedClass.Topics;
                topicsList.Items.Clear();
                foreach (string topic in topics)
                {
                    ComboBoxItem topicDispaly = new ComboBoxItem();
                    topicDispaly.Content = topic;
                    topicsList.Items.Add(topicDispaly);
                }
            }
           
        }
        private void _base_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size c = e.NewSize;
            StudentList.Height = c.Height;
            ClassList.Height = c.Height;
            toolBar.Height = c.Height;
            textPanel.Height = c.Height;
            textPanel.Width = c.Width - StudentList.Width * 2 - toolBar.Width;
            mainTextBox.Width = textPanel.Width-10;
            mainTextBox.Height = c.Height - addText.Height;
        }
        /**********************************************/
        public void refreshClassList()
        {
            foreach (CourseInfo student in selectedStudent.ClassList)
            {
                ListViewItem item = new ListViewItem();
                item.Content ="Session"+ student.SessionNumber;
                item.FontSize=20;
                ClassList.Items.Add(item);
            }
        }
        public async Task WriteDataToFileAsync(string fileName, string content)
        {
            byte[] data = Encoding.Unicode.GetBytes(content);
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
            {
                await s.WriteAsync(data, 0, data.Length);
            }
        }
        public async Task<StudentInfo[]> ReadFileAllfile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StudentInfo));
            List<StudentInfo> list = new List<StudentInfo>();
            var folder = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
           
            foreach (StorageFile file in fileList)
            {
                var contents = await this.ReadFileContentsAsync(file.Name);
                StudentInfo newStudent = (StudentInfo)serializer.Deserialize(contents);
                
                list.Add(newStudent);
                
            }
            
            return list.ToArray();
        }
        public async Task DeleteAsync(string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(fileName);
            await file.DeleteAsync();
        }
        public async Task<MemoryStream> ReadFileContentsAsync(string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                
                Stream file = await folder.OpenStreamForReadAsync(fileName);
                byte[] buffer = new byte[8192];
                file.Read(buffer, 0, buffer.Length);
                MemoryStream stream = new MemoryStream(buffer);
                using (var streamReader = new StreamReader(file))
                {
                    return stream;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async void loadStudentList()
        {
            StudentInfo[] studentLists = await this.ReadFileAllfile();
            foreach (var student in studentLists)
            {
                studentList.Add(student);
            }
            ListViewItem item = new ListViewItem();
            //textBlock2.Text = "" + studentList.Count;
            foreach (StudentInfo student in studentList)
            {
                item = new ListViewItem();
                item.Content = student.StudentName;
                item.FontSize = 20;
                StudentList.Items.Add(item);
            }
        }
        public void loadClassList()
        {
            classList = selectedStudent.ClassList;
            foreach (CourseInfo course in classList)
            {
                ListViewItem courseDisplay = new ListViewItem();
                courseDisplay.Content = "Session " + course.SessionNumber;
                ClassList.Items.Add(courseDisplay);
            }
        }
        public async void saveFile()
        {
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, selectedStudent);
            string result = System.Text.Encoding.UTF8.GetString(stream.ToArray(), 0, stream.ToArray().Length);
            await WriteDataToFileAsync(selectedStudent.StudentName + ".S", result);
        }
    }
}
