using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.IO;
using OpenQA.Selenium.Support.Events;

namespace Task_one_project
{
    [TestFixture]
    public class AdminLogin
    {
        private EventFiringWebDriver driver;

        [SetUp]
        public void Start()
        {
            driver = new EventFiringWebDriver(new ChromeDriver());
            driver.FindingElement += (sender, e) => Console.WriteLine(e.FindMethod);
            driver.FindElementCompleted += (sender, e) => Console.WriteLine(e.FindMethod + " found");
            driver.ExceptionThrown += (sender, e) => Console.WriteLine(e.ThrownException);
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

        [Test]
        public void NewProduct()
        {
            string productName = "Some product with relative image";

            Login();
            // открыть меню Catalog
            driver.Url = "http://localhost/litecart/admin/?app=catalog&doc=catalog";

            // нажать кнопку "Add New Product"
            driver.FindElement(By.LinkText("Add New Product")).Click();

            // заполнить поля с информацией о товарe: General Tab
            driver.FindElement(By.Name("status")).Click();
            driver.FindElement(By.Name("name[en]")).SendKeys(productName);
            driver.FindElement(By.Name("code")).SendKeys("123");
            driver.FindElement(By.XPath("(//input[@name='categories[]'])[2]")).Click();
            var categoryId = driver.FindElement(By.Name("default_category_id"));
            var selectCategoryId = new SelectElement(categoryId);
            selectCategoryId.SelectByText("Rubber Ducks");
            driver.FindElement(By.XPath("(//input[@name='product_groups[]'])[3]")).Click();
            driver.FindElement(By.Name("quantity")).SendKeys("50");
            var soldOutStatus = driver.FindElement(By.Name("sold_out_status_id"));
            var selectStatus = new SelectElement(soldOutStatus);
            selectStatus.SelectByText(@"-- Select --");
            driver.FindElement(By.Name("new_images[]")).SendKeys(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\..\images\otter.jpg"));
            driver.FindElement(By.Name("date_valid_from")).SendKeys(Keys.Home + "01/04/2018");
            driver.FindElement(By.Name("date_valid_to")).SendKeys(Keys.Home + "01/04/2020");

            // заполнить поля с информацией о товарe: Information Tab
            driver.FindElement(By.LinkText("Information")).Click();
            var manufacturer = driver.FindElement(By.Name("manufacturer_id"));
            var selectManufacturer = new SelectElement(manufacturer);
            selectManufacturer.SelectByText("ACME Corp.");
            driver.FindElement(By.Name("keywords")).SendKeys("Keywords");
            driver.FindElement(By.Name("short_description[en]")).SendKeys("Short Description");
            driver.FindElement(By.CssSelector(".trumbowyg-editor")).SendKeys("Description");
            driver.FindElement(By.Name("head_title[en]")).SendKeys("Head Title");
            driver.FindElement(By.Name("meta_description[en]")).SendKeys("Meta Description");

            // заполнить поля с информацией о товарe: Prices Tab
            driver.FindElement(By.LinkText("Prices")).Click();
            driver.FindElement(By.Name("purchase_price")).SendKeys(Keys.Home + "10");
            var currency = driver.FindElement(By.Name("purchase_price_currency_code"));
            var selectCurrency = new SelectElement(currency);
            selectCurrency.SelectByText("Euros");
            driver.FindElement(By.Name("gross_prices[EUR]")).SendKeys(Keys.Home + "15");

            // сохранить
            driver.FindElement(By.Name("save")).Click();

            // появился в каталоге (в админке)
            driver.FindElement(By.LinkText(productName));
        }

        [Test]
        public void ProductCart()
        {
            // повторим 3 раза, чтобы в корзине было 3 товара
            for (int i = 0; i < 3; i++)
            {
                // открыть главную страницу
                driver.Url = "http://localhost/litecart";

                // открыть первый товар из списка
                driver.FindElement(By.CssSelector(".product")).Click();
                int cartCountBefore = int.Parse(driver.FindElement(By.CssSelector(".quantity")).GetAttribute("textContent"));

                // добавить его в корзину(при этом может случайно добавиться товар, который там уже есть, ничего страшного)
                driver.FindElement(By.Name("add_cart_product")).Click();

                // подождать, пока счётчик товаров в корзине обновится
                string cartCountAfter = (cartCountBefore + 1).ToString();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector(".quantity"), cartCountAfter));
            }
 
            // открыть корзину(в правом верхнем углу кликнуть по ссылке Checkout)
            driver.FindElement(By.LinkText("Checkout »")).Click();

            // удалить все товары из корзины один за другим, после каждого удаления подождать, пока внизу обновится таблица
            for (int i = 0; i < 3; i++)
            {
                driver.FindElement(By.Name("remove_cart_item")).Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("remove_cart_item")));
            }
        }

        [Test]
        public void NewWindows()
        {
            // зайти в админку
            Login();

            // открыть пункт меню Countries(или страницу http://localhost/litecart/admin/?app=countries&doc=countries)
            driver.Url = "http://localhost/litecart/admin/?app=countries&doc=countries";

            // открыть на редактирование какую-нибудь страну или начать создание новой
            driver.FindElement(By.LinkText("Add New Country")).Click();
            string countriesWindow = driver.CurrentWindowHandle;

            IList<string> handles = driver.WindowHandles;

            /* возле некоторых полей есть ссылки с иконкой в виде квадратика со стрелкой --
               они ведут на внешние страницы и открываются в новом окне, именно это и нужно проверить.
               Конечно, можно просто убедиться в том, что у ссылки есть атрибут target = "_blank".
            */

            ReadOnlyCollection<IWebElement> externalLinks = driver.FindElements(By.CssSelector(".fa-external-link"));

            // повторить эти действия для всех таких ссылок.
            for (int i = 0; i < externalLinks.Count; i++)
            {
                IWebElement externalLink = externalLinks[i];

                // Но в этом упражнении требуется именно кликнуть по ссылке, чтобы она открылась в новом окне,
                externalLink.Click();

                // новое окно открывается не мгновенно, поэтому требуется ожидание открытия окна.
                Func<IWebDriver, string> ThereIsWindowOtherThan = new Func<IWebDriver, string>((IWebDriver Web) =>
                {
                    IList<string> curHandles = driver.WindowHandles.ToList();
                    for (int j = 0; j < handles.Count; j++)
                    {
                        curHandles.Remove(handles[j]);
                    }
                    if (curHandles.Count != 0)
                    {
                        return curHandles[0];
                    }
                    return null;

                });

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                string externalWindow = wait.Until(ThereIsWindowOtherThan);

                // потом переключиться в новое окно, 
                driver.SwitchTo().Window(externalWindow);

                //закрыть его, 
                driver.Close();

                //вернуться обратно
                driver.SwitchTo().Window(countriesWindow);
            }
        }

        [Test]
        public void Events()
        {
            // зайти в админку
            Login();

            // открыть каталог, категорию, которая содержит товары
            driver.Url = "http://localhost/litecart/admin/?app=catalog&doc=catalog&category_id=1";

            // последовательно открывать страницы товаров и проверять, не появляются ли в логе браузера сообщения(любого уровня)
            ReadOnlyCollection<IWebElement> products = driver.FindElements(By.CssSelector(".dataTable tr"));
            List<string> links = new List<string>();
            List<bool> isFolder = new List<bool>();
            for(int i = 2; i < products.Count-1; i++)
            {
                IWebElement product = products[i];

                links.Add(product.FindElements(By.CssSelector("td"))[2]
                        .FindElement(By.CssSelector("a")).GetAttribute("href"));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                isFolder.Add(product.FindElements(By.CssSelector(".fa-folder")).Count != 0);
            }
            
            for (int i = 0; i < products.Count-3; i++)
            {
                driver.Url = links[i];

                if (isFolder[i])
                { 
                    string subProductUrl = driver.Url;

                    ReadOnlyCollection<IWebElement> subProducts = driver.FindElements(By.CssSelector(".dataTable tr"));
                    for(int j = 0; j < subProducts.Count - products.Count; j++)
                    {
                        subProducts[i+j+3].FindElements(By.CssSelector("td"))[2]
                        .FindElement(By.CssSelector("a")).Click();
                       
                        driver.Url = subProductUrl;
                        subProducts = driver.FindElements(By.CssSelector(".dataTable tr"));
                    }
                }
                driver.Url = "http://localhost/litecart/admin/?app=catalog&doc=catalog&category_id=1";
            }
            foreach (LogEntry l in driver.Manage().Logs.GetLog("browser"))
            {
                Console.WriteLine(l);
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