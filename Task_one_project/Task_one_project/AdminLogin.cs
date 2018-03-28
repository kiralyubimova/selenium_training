﻿using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

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

        public string GetRandomString(Random random, int length)
        {
            char[] chars = "abcdefghijklmnopqrstuvw0987654321".ToCharArray();
            string str = "";
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(0, chars.Length - 1);
                str += chars[index];
            }
            return str;
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
        public void ProductsPage()
        {
            //Main Page
            driver.Url = "http://localhost/litecart";
            IWebElement productCampaigns = driver.FindElement(By.CssSelector("#box-campaigns .link"));

            string productNameMainPage = productCampaigns.FindElement(By.CssSelector(".name")).GetAttribute("textContent");

            string productPriceRegularMainPage = productCampaigns.FindElement(By.CssSelector(".regular-price")).GetAttribute("textContent");
            float regularPriceMain = float.Parse(productPriceRegularMainPage.Replace("$", ""));
            string textStyle = productCampaigns.FindElement(By.CssSelector(".regular-price")).GetCssValue("text-decoration-line");
            string textSizeRegular = productCampaigns.FindElement(By.CssSelector(".regular-price")).GetCssValue("font-size");
            float fontSizeRegular = float.Parse(textSizeRegular.Replace("px", ""));
            string textColor = productCampaigns.FindElement(By.CssSelector(".regular-price")).GetCssValue("color");
            string[] numbers = textColor.Replace("rgba(", "").Replace(")", "").Split(',');
            int r = int.Parse(numbers[0].Trim());
            int g = int.Parse(numbers[1].Trim());
            int b = int.Parse(numbers[2].Trim());
            Assert.AreEqual(textStyle, "line-through");
            Assert.IsTrue(r == g && r == b);

            string productPriceCampaignMainPage = productCampaigns.FindElement(By.CssSelector(".campaign-price")).GetAttribute("textContent");
            float campaignPriceMain = float.Parse(productPriceCampaignMainPage.Replace("$", ""));
            int textStyleCampaign = int.Parse(productCampaigns.FindElement(By.CssSelector(".campaign-price")).GetCssValue("font-weight"));
            string textSizeCampaign = productCampaigns.FindElement(By.CssSelector(".campaign-price")).GetCssValue("font-size");
            float fontSizeCampaign = float.Parse(textSizeCampaign.Replace("px", ""));
            string textColorCampaign = productCampaigns.FindElement(By.CssSelector(".campaign-price")).GetCssValue("color");
            string[] numbersCampaigns = textColorCampaign.Replace("rgba(", "").Replace(")", "").Split(',');
            r = int.Parse(numbersCampaigns[0].Trim());
            g = int.Parse(numbersCampaigns[1].Trim());
            b = int.Parse(numbersCampaigns[2].Trim());
            Assert.IsTrue(textStyleCampaign > 400);
            Assert.IsTrue(g == 0 && b == 0);

            Assert.IsTrue(fontSizeRegular < fontSizeCampaign);

            //Product's Page
            productCampaigns.Click();
            IWebElement productPage = driver.FindElement(By.CssSelector("#box-product"));

            string productNameProductPage = productPage.FindElement(By.CssSelector("h1.title")).GetAttribute("textContent");
            Assert.AreEqual(productNameMainPage, productNameProductPage);

            string productPriceRegularProductPage = productPage.FindElement(By.CssSelector(".regular-price")).GetAttribute("textContent");
            float regularPriceProduct = float.Parse(productPriceRegularProductPage.Replace("$", ""));
            Assert.AreEqual(regularPriceMain, regularPriceProduct);

            string productPriceCampagnProductPage = productPage.FindElement(By.CssSelector(".campaign-price")).GetAttribute("textContent");
            float campagnPriceProduct = float.Parse(productPriceCampagnProductPage.Replace("$", ""));
            Assert.AreEqual(campaignPriceMain, campagnPriceProduct);

            string productTextStyle = productPage.FindElement(By.CssSelector(".regular-price")).GetCssValue("text-decoration-line");
            Assert.AreEqual(textStyle, "line-through");

            string productTextColor = productPage.FindElement(By.CssSelector(".regular-price")).GetCssValue("color");
            string[] colors = textColor.Replace("rgba(", "").Replace(")", "").Split(',');
            r = int.Parse(colors[0].Trim());
            g = int.Parse(colors[1].Trim());
            b = int.Parse(colors[2].Trim());
            Assert.IsTrue(r == g && r == b);

            int productTextStyleCampaign = int.Parse(productPage.FindElement(By.CssSelector(".campaign-price")).GetCssValue("font-weight"));
            Assert.IsTrue(productTextStyleCampaign > 400);

            string productTextColorCampaign = productPage.FindElement(By.CssSelector(".campaign-price")).GetCssValue("color");
            string[] colorsCampaigns = textColorCampaign.Replace("rgba(", "").Replace(")", "").Split(',');
            r = int.Parse(colorsCampaigns[0].Trim());
            g = int.Parse(colorsCampaigns[1].Trim());
            b = int.Parse(colorsCampaigns[2].Trim());
            Assert.IsTrue(g == 0 && b == 0);

            string productTextSizeRegular = productPage.FindElement(By.CssSelector(".regular-price")).GetCssValue("font-size");
            float productFontSizeRegular = float.Parse(productTextSizeRegular.Replace("px", ""));
            string productTextSizeCampaign = productPage.FindElement(By.CssSelector(".campaign-price")).GetCssValue("font-size");
            float productFontSizeCampaign = float.Parse(productTextSizeCampaign.Replace("px", ""));
            Assert.IsTrue(productFontSizeRegular < productFontSizeCampaign);
        }

        [Test]
        public void NewUser()
        {
            // регистрация новой учётной записи с достаточно уникальным адресом электронной почты (чтобы не конфликтовало с ранее созданными пользователями, в том числе при предыдущих запусках того же самого сценария),
            driver.Url = "http://localhost/litecart";
            driver.FindElement(By.LinkText("New customers click here")).Click();
            driver.FindElement(By.Name("tax_id")).SendKeys("12345");
            driver.FindElement(By.Name("company")).SendKeys("ABC");
            driver.FindElement(By.Name("firstname")).SendKeys("John");
            driver.FindElement(By.Name("lastname")).SendKeys("Smith");
            driver.FindElement(By.Name("address1")).SendKeys("Elm street 12345");
            driver.FindElement(By.Name("address2")).SendKeys("Oak street 54321");
            driver.FindElement(By.Name("postcode")).SendKeys("10007");
            driver.FindElement(By.Name("city")).SendKeys("New York");
            var country = driver.FindElement(By.Name("country_code"));
            var selectElement = new SelectElement(country);
            selectElement.SelectByValue("US");
            IWebElement countryZone = driver.FindElements(By.CssSelector("tr"))[4]
                .FindElements(By.CssSelector("td"))[1]
                .FindElement(By.CssSelector("select"));
            var selectState = new SelectElement(countryZone);
            selectState.SelectByValue("AR");
            string emailAddress = GetRandomString(new Random(), 6);
            driver.FindElement(By.Name("email")).SendKeys(emailAddress + "@eee.eee");
            driver.FindElement(By.Name("phone")).SendKeys("1234567890");
            driver.FindElement(By.Name("password")).SendKeys("abc1");
            driver.FindElement(By.Name("confirmed_password")).SendKeys("abc1");
            driver.FindElement(By.Name("create_account")).Click();

            // выход(logout), потому что после успешной регистрации автоматически происходит вход,
            driver.FindElement(By.LinkText("Logout")).Click();

            //повторный вход в только что созданную учётную запись,
            driver.FindElement(By.Name("email")).SendKeys(emailAddress + "@eee.eee");
            driver.FindElement(By.Name("password")).SendKeys("abc1");
            driver.FindElement(By.Name("login")).Click();

            // и ещё раз выход.
            driver.FindElement(By.LinkText("Logout")).Click();
        }

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}