using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneratorBarcode.Domain;
using System.Linq;

namespace GeneratorBarcode.Test
{
    [TestClass]
    public class ConvenienceStoreBarcodeTest
    {
        [TestMethod]
        [DataRow("2020-01-04")]
        [DataRow("2020/01/04")]
        public void 計算繳費期限加上19天等於訂單日期(String data)
        {
            // arrange
            var cs = new ConvenienceStoreBarcode();
            DateTime create_datetime = DateTime.Parse(data);

            // act
            DateTime paymentDeadine = cs.GetPaymentDeadline(create_datetime);

            // assert
            Assert.AreEqual(paymentDeadine, create_datetime.AddDays(16));

        }

        [TestMethod]
        [DataRow("2019-04-16")]
        [DataRow("2019/04/16")]
        public void 西元年轉為民國年(String data)
        {
            // arrange
            var cs = new ConvenienceStoreBarcode();
            DateTime create_datetime = DateTime.Parse(data);

            // act
            string result = cs.TranYearToChineseYear(create_datetime);

            // assert
            Assert.AreEqual(result, "01080416");

        }

        [TestMethod]
        public void 產生超商條碼產生器()
        {
            //arrange
            //act
            var cs = new ConvenienceStoreBarcode("ZWB16062087521292101", "I0F",
                DateTime.Parse("2016-06-20 13:09:09.910"),
                3000);

            //assert
            Assert.AreEqual(cs.OrderSeq, "ZWB16062087521292101");
            Assert.AreEqual(cs.SuffixCode, "I0F");
            Assert.AreEqual(cs.Premium, 3000);
            
        }

        [TestMethod]
        public void 產生第一列條碼()
        {
            //第一段(9)：繳費期限國曆後6位 + I0F / I01
            //arrange
            var cs = new ConvenienceStoreBarcode("99999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            int[] result = cs.GeneratorOne();

            //assert
            int[] actual = { 0, 8, 0, 4, 1, 6, 9, 0, 6 };

            result.SequenceEqual(actual);
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(result[i], actual[i]);
            }

        }

        [TestMethod]
        public void 產生第二列條碼()
        {
            //第二段(16)：訂單號碼前16位
            //arrange
            var cs = new ConvenienceStoreBarcode("99999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            int[] result = cs.GeneratorTwo();

            //assert
            int[] actual = { 9 , 9  , 9 ,  9  , 9  , 9 ,  9 ,  9 ,  9 ,  9  , 0 ,  3 ,  1 ,  6 ,  0  , 0 };

            for(int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(result[i], actual[i]);
            }

        }

        [TestMethod]
        public void 產生第三列條碼()
        {
            //第三段(15)：訂單號碼第17~20位 +檢查碼(2)+金額(9)

            //arrange
            var cs = new ConvenienceStoreBarcode("99999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            int[] result = cs.GeneratorThree();

            //assert
            int[] actual = { 0 , 4  , 0 ,  9  , 0 ,  0  , 0 ,  0  , 0 ,  0 ,  1 ,  1 ,  7,   0 ,  0 };

            result.SequenceEqual(actual);
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(result[i], actual[i]);
            }

        }

        [TestMethod]
        public void 取得檢查碼()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("99999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            var result = cs.CheckCode;

            int[] acutal = { 4, 9 };

            result.SequenceEqual(acutal);

            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
        }

        [TestMethod]
        public void 取得檢查碼_餘數為0()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("50999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            var result = cs.CheckCode;

            int[] acutal = { 0, 0 };

            result.SequenceEqual(acutal);

            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
        }

        [TestMethod]
        public void 取得檢查碼_餘數為10()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("50086999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            var result = cs.CheckCode;

            int[] acutal = { 10, 10 };

            result.SequenceEqual(acutal);

            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
        }

        [TestMethod]
        public void 取得條碼資料_Excel範例()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("99999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = {"080416906","9999999999031600","040949000011700"};

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_檢核碼餘數為0()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("50999999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "080416906", "5099999999031600", "0409AX000011700" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_檢核碼餘數為10()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("50086999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "080416906", "5008699999031600", "0409BY000011700" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_測試資料_ZWB18072872194892201()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("ZWB18072872194892201", "I0F",
                    DateTime.Parse("2018-07-28 10:46:00"),
                    4628);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "070813906", "9621807287219489", "220182000004628" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_使用方法取_測試資料_ZWB18072872194892201()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode();
            //act
            string[] result = cs.GetBarcode("ZWB18072872194892201", "I0F",
                    DateTime.Parse("2018-07-28 10:46:00"),
                    4628);

            //actual
            string[] acutal = { "070813906", "9621807287219489", "220182000004628" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }


    }
}
