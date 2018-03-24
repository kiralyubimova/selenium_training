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
        public void AlphabeticalOrder_1()
        {
            Login();

            driver.Url = "http://localhost/litecart/admin/?app=countries&doc=countries";
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.CssSelector(".row"));
            List<CountryData> countries = new List<CountryData>();
            for (int i = 0; i < rows.Count; i++)
            {
                IWebElement countryRow = rows[i];
                ReadOnlyCollection<IWebElement> columns = countryRow.FindElements(By.CssSelector("td"));
                CountryData newCountry = new CountryData(columns[4].GetAttribute("textContent"));
                newCountry.Code = columns[3].GetAttribute("textContent");
                newCountry.ZonesCount = int.Parse(columns[5].GetAttribute("textContent"));
                newCountry.Link = columns[4].FindElement(By.CssSelector("a")).GetAttribute("href");
                countries.Add(newCountry);
            }
            List<CountryData> countriesOrdered = new List<CountryData>();
            for(int i = 0; i < countries.Count; i++)
            {
                countriesOrdered.Add(countries[i]);
            }
            countriesOrdered.Sort();
            for(int i = 0; i < countries.Count; i++)
            {
                Assert.True(countries[i] == countriesOrdered[i]);
            }

            foreach(CountryData country in countries)
            {
                if (country.ZonesCount != 0)
                {
                    driver.Url = country.Link;
                    ReadOnlyCollection<IWebElement> zonesTable = driver.FindElements(By.CssSelector("#table-zones tr"));
                    List<string> zonesSorted = new List<string>();
                    for (int i = 1; i < zonesTable.Count-1; i++) //first element with index == 0 is header and we skip it. last element contents input fields for new zone
                    {
                        string zoneName = zonesTable[i].FindElements(By.CssSelector("td"))[2].GetAttribute("textContent");
                        country.Zones.Add(zoneName);
                        zonesSorted.Add(zoneName);
                    }
                    Assert.IsTrue(country.ZonesCount == country.Zones.Count);
                    zonesSorted.Sort();
                    for(int i = 0; i < zonesSorted.Count; i++)
                    {
                        Assert.IsTrue(zonesSorted[i] == country.Zones[i]);
                    }
                }
            }

        }

        [Test]
        public void AlphabeticalOrderGeoZonesPage()
        {
            Login();
            driver.Url = "http://localhost/litecart/admin/?app=geo_zones&doc=geo_zones";
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.CssSelector(".row"));
            List<CountryData> countries = new List<CountryData>();
            for (int i = 0; i < rows.Count; i++)
            {
                IWebElement countryRow = rows[i];
                ReadOnlyCollection<IWebElement> columns = countryRow.FindElements(By.CssSelector("td"));
                CountryData newCountry = new CountryData(columns[2].GetAttribute("textContent"));
                newCountry.Link = columns[2].FindElement(By.CssSelector("a")).GetAttribute("href");
                countries.Add(newCountry);
            }
            foreach (CountryData country in countries)
            {
                driver.Url = country.Link;
                ReadOnlyCollection<IWebElement> zones = driver.FindElements(By.CssSelector("#table-zones tr"));
                List<string> zonesList = new List<string>();
                List<string> zonesListSorted = new List<string>();
                for (int i = 1; i < zones.Count-1; i++)
                {
                    string zoneSelected = zones[i].FindElements(By.CssSelector("td"))[2]
                        .FindElement(By.CssSelector("[selected]")).GetAttribute("textContent");
                    zonesList.Add(zoneSelected);
                    zonesListSorted.Add(zoneSelected);
                }
                zonesListSorted.Sort();
                for (int i = 0; i < zonesList.Count; i++)
                {
                    Assert.IsTrue(zonesList[i] == zonesListSorted[i]);
                }
            }
        }

        [Test]
        public void GoodsPage()
        {
            driver.Url = "http://localhost/litecart";
            IWebElement goodCampaigns = driver.FindElement(By.CssSelector("#box-campaigns .link"));

            //Main Page
            string goodNameMainPage = goodCampaigns.FindElement(By.CssSelector(".name")).GetAttribute("textContent");

            string goodPriceRegularMainPage = goodCampaigns.FindElement(By.CssSelector(".regular-price")).GetAttribute("textContent");
            string textStyle = goodCampaigns.FindElement(By.CssSelector(".regular-price")).GetCssValue("text-decoration-line");
            string textColor = goodCampaigns.FindElement(By.CssSelector(".regular-price")).GetCssValue("color");

            string[] numbers = textColor.Replace("rgb(", "").Replace(")", "").Split(',');
            int r = int.Parse(numbers[0].Trim());
            int g = int.Parse(numbers[1].Trim());
            int b = int.Parse(numbers[2].Trim());

            string goodPriceCampaignMainPage = goodCampaigns.FindElement(By.CssSelector(".campaign-price")).GetAttribute("textContent");

            Assert.AreEqual(textStyle, "line-through");
        }

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}