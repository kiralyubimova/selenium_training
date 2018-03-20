using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.ObjectModel;
using System.Linq;

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

        [Test]
        public void AdminPageLogin()
        {
            driver.Url = "http://localhost/litecart/admin/";
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Click();
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

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}