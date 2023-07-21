using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees;
            List<Employee> startdata;
            using StreamReader reader2 = new("./JsonFiles/StartData.json");
            {
                var json = reader2.ReadToEnd();
                startdata = JsonConvert.DeserializeObject<List<Employee>>(json);
            }

            using StreamReader reader = new("./JsonFiles/Employees.json");
            {
                var json = reader.ReadToEnd();
                employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                employees.ForEach(a => a.NewData = true);
            }
           
            startdata.AddRange(employees);
            var arrayforresult = startdata.GroupBy(a => a.Login);
            List<Employee> result = new List<Employee>();

            foreach (var element in arrayforresult)
            {
                var el = element.Where(a => a.Date_hired == element.Max(b => b.Date_hired)); // берем сотрудника с максимальной датой найма
                if (el.Count() > 1)
                {
                    el = el.Where(a => a.Date_fired is not null);
                }
                else if (el.First().Date_fired is not null)
                {
                    if(el.Where(a => !a.NewData).Count() == 0)
                    {
                        continue; // если записи с уволенным не было, то пропускаеи
                    }
                }
                var res = el.First();
                if (res.Date_fired is not null)
                    res.Disabled = true;
                result.Add(res);
            }

            using (StreamWriter writer = new StreamWriter(new FileStream("./JsonFiles/FinalData.json", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            Console.WriteLine("файл FinalData.json записан в папке дебага");
        }
    }
    
    class Employee
    {
        public string Login { get; set; }
        public int PersonId { get; set; }
        public string Specialization { get; set; }
        public DateTime? Date_hired { get; set; }
        public DateTime? Date_fired { get; set; }
        public bool Disabled { get; set; }
        [JsonIgnore]
        public bool NewData { get; set; } = false;
    }
}
