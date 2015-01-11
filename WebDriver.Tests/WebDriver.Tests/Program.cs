using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace WebDriver.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("http://www.google.com");
            try
            {
                if (driver.FindElement(By.Id("hplogo")).Displayed)
                {
                    Console.WriteLine("Yeah this is working!!");
                    var abc = Console.Read();
                }
            }
            catch(Exception)
            {
                Console.WriteLine("asdfasdf");
                var abc = Console.Read();
            }
            
            //driver.Close();
        }
    }
}
