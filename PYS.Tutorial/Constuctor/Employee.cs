using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PYS.Tutorial.Constuctor
{
    public class Employee
    {
        public int salary;

        public Employee(int annalSalary)
        {
            salary = annalSalary;
        }

        public Employee(int weeklySalary, int numberOfWeeks)
        {
            salary = weeklySalary * numberOfWeeks;
        }
    }
}
