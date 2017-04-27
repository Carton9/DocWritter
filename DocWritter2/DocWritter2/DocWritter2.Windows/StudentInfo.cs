using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DocWritter2
{
    [XmlRoot("Student")]
    public class StudentInfo
        {
            String studentName;
            DateTime birth;
            String classType;
            String gen;
            List<CourseInfo> classList = new List<CourseInfo>();
            public string StudentName
            {
                get { return studentName; }
                set { studentName = value; }
            }
            public DateTime Birth
            {
                get { return birth; }
                set { birth = value; }
            }
            public string Gen
            {
                get { return gen; }
                set { gen = value; }
             }
            public string ClassType
            {
                get { return classType; }
                set { classType = value; }
            }
            public List<CourseInfo> ClassList
            {
                get { return classList; }
                set { classList = value; }
            }
        }
        public class CourseInfo
        {
            List<string> topics = new List<string>();
            List<string> texts = new List<string>();
            DateTime date;
            TimeSpan start;
            TimeSpan end;
            Boolean isFinish;
            int sessionNumber;
            public int SessionNumber
            {
                get { return sessionNumber; }
                set { sessionNumber = value; }
            }
            public TimeSpan Start
            {
                get { return start; }
                set { start = value; }
            }
            public TimeSpan End
            {
                get { return end; }
                set { end = value; }
            }
            public DateTime Date
            {
                get { return date; }
                set { date = value; }
            }
            public List<string> Topics
            {
                get { return topics; }
                set { topics = value; }
            }
            public List<string> Texts
            {
                get { return texts; }
                set { texts = value; }
            }


            [XmlIgnore]
            public Boolean Finish
            {
                get { return isFinish; }
                set { isFinish = value; }
            }
            public void addTopic(string topicName, string text)
            {
                if (!isFinish)
                {
                    topics.Add(topicName);
                    texts.Add(text);
                }
            }
            public void changeTopic(string topicName, string text)
            {
                if (!isFinish)
                {
                    if (topics.IndexOf(topicName) == -1) addTopic(topicName, text);
                    else texts.Insert(topics.IndexOf(topicName), text);
                }

            }
            public void addSummary(string text)
            {
                addTopic("Summary", text);
                isFinish = true;
            }
        }

}
