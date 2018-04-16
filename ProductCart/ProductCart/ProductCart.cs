using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ProductCart
{
    [TestFixture]
    public class ProductCartTest
    {
        private ChromeDriver driver;

        [SetUp]
        public void Start()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
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

        [TearDown]
        public void Stop()
        {
            driver.Quit();
            driver = null;
        }
    }
}