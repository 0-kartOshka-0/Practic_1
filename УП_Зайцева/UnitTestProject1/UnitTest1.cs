using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using УП_Зайцева;

namespace UnitTestProject1
{
    [TestClass]
    public class AuthorizationTests
    {
        private Authorization _authorization;
        private string _connectionString = "Data Source=ADCLG1;Initial Catalog=!!!Зайцева_УП;Integrated Security=true";

        [TestInitialize]
        public void Setup()
        {
            _authorization = new Authorization();
        }

        [TestMethod]
        public void GetUserRole_ReturnsRole_WhenUserExists()
        {
            string username = "Тестовый Пользователь"; 
            string expectedRole = "Командир"; 
            string result = _authorization.GetUserRole(username, _connectionString);
            Assert.AreEqual(expectedRole, result);
        }

        [TestMethod]
        public void GetUserRole_ReturnsRole_WhenUserExists2()
        {
            string username = "Зайцева Дарья Александровна";
            string expectedRole = "Инстпектор";
            string result = _authorization.GetUserRole(username, _connectionString);
            Assert.AreEqual(expectedRole, result);
        }

        [TestMethod]
        public void GetUserRole_ReturnsUnknown_WhenUserDoesNotExist()
        {
            string result = _authorization.GetUserRole("НеСуществующийПользователь", _connectionString);
            Assert.AreEqual("Unknown", result);
        }

        [TestMethod]
        public void AuthenticateUser_ReturnsTrue_WhenValidCredentials()
        {
            string username = "Тестовый Пользователь";
            string password = "2222";
            bool result = _authorization.AuthenticateUser(username, password, _connectionString);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AuthenticateUser_ReturnsTrue_WhenValidCredentials2()
        {
            string username = "Кузнецов Николай Николаевич"; 
            string password = "89003456789";
            bool result = _authorization.AuthenticateUser(username, password, _connectionString);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void AuthenticateUser_ReturnsFalse_WhenInvalidCredentials()
        {
            string username = "invalidUser";
            string password = "invalidPassword";
            bool result = _authorization.AuthenticateUser(username, password, _connectionString);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void AuthenticateUser_ReturnsFalse_WhenInvalidCredentials2()
        {
            string username = "Петров Петр Петрович";
            string password = "1111";
            bool result = _authorization.AuthenticateUser(username, password, _connectionString);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserID_ReturnsUserID_WhenUserExists()
        {
            string username = "Тестовый Пользователь";
            int result = _authorization.GetUserID(username, _connectionString);
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void GetUserID_ReturnsMinusOne_WhenUserDoesNotExist()
        {
            int result = _authorization.GetUserID("НеСуществующийПользователь", _connectionString);
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void Button2_Click_ShowsMessage_OnEmptyFields()
        {
            var textBox1 = new TextBox();
            var textBox2 = new TextBox();
            _authorization.Controls.Add(textBox1);
            _authorization.Controls.Add(textBox2);
            _authorization.button2_Click(null, null);
            }
    }
}