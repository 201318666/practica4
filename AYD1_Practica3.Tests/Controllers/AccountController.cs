﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AYD1_Practica3;
using AYD1_Practica3.Models;
using AYD1_Practica3.Controllers;

namespace AYD1_Practica3.Tests.Controllers
{
    [TestClass]
    public class TransferenciaControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        // 
        [TestMethod]
        public void ExisteDestino()
        {
            //Arrange
            AccountController controller = new AccountController();

            //Act
            bool existe = controller.ExisteDestino("DTQ5Q");

            // assert 
            Assert.IsFalse(existe, "Numero de cuenta destino no existe");
        }

        // 
        [TestMethod]
        public void HayFondos()
        {
            //Arrange
            AccountController controller = new AccountController();

            //Act
            bool existe = controller.HayFondos("Javier", 1.2);

            // assert 
            Assert.IsFalse(existe, "Cuenta destino no tiene fondos");
        }

    }
}