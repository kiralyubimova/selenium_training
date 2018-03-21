using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Task_one_project
{
    [TestFixture]
    public class AdminLogin
    {
        private IWebDriver driver;

        [SetUp]
        public void Start()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public bool IsElementPresent(By locator)
        {
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void Login()
        {
            driver.Url = "http://localhost/litecart/admin/";
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Click();
        }

        [Test]
        public void AdminPageLogin()
        {
            Login();
            ReadOnlyCollection<IWebElement> menu = driver.FindElements(By.CssSelector("#app- > a"));
            for(int i = 0; i < menu.Count; i++)
            {
                IWebElement listItem = menu[i];
                listItem.Click();
                ReadOnlyCollection<IWebElement> menu2 = driver.FindElements(By.CssSelector("#app- > li > a"));
                if (menu2.Count != 0)
                {
                    foreach(IWebElement el in menu2)
                    {
                        el.Click();
                        Assert.True(IsElementPresent(By.CssSelector("h1")));
                    }
                }
                else
                {
                    Assert.True(IsElementPresent(By.CssSelector("h1")));
                }
                menu = driver.FindElements(By.CssSelector("#app- > a"));

            }
        }

        [Test]
        public void StickerCheck()
        {
            driver.Url = "http://localhost/litecart";
            ReadOnlyCollection<IWebElement> goods = driver.FindElements(By.CssSelector(".product"));
            for (int i = 0; i < goods.Count; i++)
            {
                IWebElement listItem = goods[i];
                ReadOnlyCollection<IWebElement> stickers = listItem.FindElements(By.ClassName("sticker"));
                Assert.True(stickers.Count() == 1);
            }
        }

        [Test]
        public void AlphabeticOrder_1()
        {
            Login();

            driver.Url = "http://localhost/litecart/admin/?app=countries&doc=countries";
            ReadOnlyCollection<IWebElement> countriesLink = driver.FindElements(By.CssSelector(".dataTable > a"));
            List<string> countriesTexts = new List<string>();
            for (int i = 0; i < countriesLink.Count; i++)
            {
                IWebElement countryLink = countriesLink[i];
                countriesTexts.Add(countryLink.GetAttribute("textContent"));
            }
            List<string> countriesTextsOrdered = new List<string>();
            for(int i = 0; i < countriesTexts.Count; i++)
            {
                countriesTextsOrdered.Add(countriesTexts[i]);
            }
            countriesTextsOrdered.Sort();
            for(int i = 0; i < countriesTexts.Count; i++)
            {
                Assert.True(countriesTexts[i] == countriesTextsOrdered[i]);
            }

            ReadOnlyCollection<IWebElement> zones = driver.FindElements(By.CssSelector(".dataTable td:nth-of-type(6)"));
            int zone = new int();
            for (int i = 0; i < zones.Count; i++)
            {
                zone = int.Parse(zones[i].GetAttribute("textContent"));
            }
            if (zone > 0)
            {

            }
        }

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}