using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Task_one_project
{
    [TestFixture]
    public class VisitPage
    {
        private IWebDriver driver;

        [SetUp]
        public void Start()
        {
            driver = new FirefoxDriver();
        }

        [Test]
        public void GoToPage()
        {
            driver.Url = "https://en.wikipedia.org/wiki/Main_Page";
        }

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}
