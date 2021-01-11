using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneratorBarcode.Domain;
using System.Linq;
using System.Collections.Generic;

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
            (int[] resultInt,string[] resultString) = cs.GeneratorOne();

            //assert
            (int[] actualInt,string[] actualString) = Tuple.Create(new int[]{0, 8, 0, 4, 1, 6, 9, 0, 6 },new string[]{ "0", "8", "0", "4", "1", "6", "I", "0", "F" });

            for (int i = 0; i < actualInt.Length; i++)
            {
                Assert.AreEqual(resultInt[i], resultInt[i]);
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
            (int[] resultInt, string[] resultString) = cs.GeneratorTwo();

            //assert
            (int[] actualInt, string[] actualString) = Tuple.Create(
                new int[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 3, 1, 6, 0, 0 }, 
                new string[] {"9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "0", "3", "1", "6", "0", "0"});

            for(int i = 0; i < actualInt.Length; i++)
            {
                Assert.AreEqual(resultInt[i], actualInt[i]);
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
            (int[] resultInt, string[] resultString) = cs.GeneratorThree();

            //assert
            int[] actual = { 0 , 4  , 0 ,  9  , 0 ,  0  , 0 ,  0  , 0 ,  0 ,  1 ,  1 ,  7,   0 ,  0 };

            resultInt.SequenceEqual(actual);
            for (int i = 0; i < resultInt.Length; i++)
            {
                Assert.AreEqual(resultInt[i], actual[i]);
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

            int[] acutal = { 6, 3 };

            result.SequenceEqual(acutal);

            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
        }

        [TestMethod]
        public void 取得檢查碼_餘數為0()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("30949999990316000409", "I0F",
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
            var cs = new ConvenienceStoreBarcode("30036999990316000409", "I0F",
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
                    DateTime.Parse("2019-04-16 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "080416I0F", "9999999999031600","040949000011700"};

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_檢核碼餘數為0()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("30949999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "080331I0F", "3094999999031600", "0409AX000011700" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_檢核碼餘數為10()
        {
            //arrange
            var cs = new ConvenienceStoreBarcode("30036999990316000409", "I0F",
                    DateTime.Parse("2019-03-31 13:09:09.910"),
                    11700);
            //act
            string[] result = cs.GetBarcode();

            //actual
            string[] acutal = { "080331I0F", "3003699999031600", "0409BY000011700" };

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
            string[] acutal = { "070728I0F", "ZWB1807287219489", "220196000004628" };

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
            string[] acutal = { "070728I0F", "ZWB1807287219489", "220196000004628" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }

        [TestMethod]
        public void 取得條碼資料_使用方法取_測試資料_URL範例()
        {
            //url
            https://officialpayment.hotains.com.tw/MarketBarcode?info=S%2B21TquhoTBh5rhty6wh4/uVMHlovWcRtBrDmBGM%2BkH39xXuA21GC5lB3cJIaWr1cpqAC8U0N0j8qqoGQUv0lFUkxYTztphYUyA3Z1g6UKeZ1T/Yhvt7aZEX9Id9XzZAKp4xrYMZ9/X0WiCG%2BNjDdhDss3Ukk07XCAJN8WjkgXHUpa5DLyNkwH4gf47vB8dtTkhKHtsiKKKnT2wBiLkUYCYnjy9zEP5tc3xRJh6dsK3%2Bdv7MJnZXpEniHxSRsJgRMmbNWHFCVazlznlpn7ui5PnNnQN/eBQ9wSGehkjN4zswJjvpFDwBpRTyoLCSm%2BNtPkZBeewecop/XnSOVJWIJLo9WogDK0Ka%2B0Fk4TLGCatIrkNt18vM6%2BmN1uDbyIgwO4xaGUmRF%2BS/qarGkKbGAwDDfXOLYAYr3d6XNF%2Bv4srY/qSY0Bc/2Br/dbv5ErSEGweu6KU/0yUwJo951D2cqvy4tzmM2OxTZO9kwCfuDufvlOwYYlmAQvM1gVHP6x/I

            //arrange
            var cs = new ConvenienceStoreBarcode();
            //act
            string[] result = cs.GetBarcode("V9903049608440024032", "I01",
                    DateTime.Parse("2020-12-06"),
                    13815);

            //actual
            string[] acutal = { "091206I01", "V990304960844002", "403253000013815" };

            result.SequenceEqual(acutal);
            Assert.AreEqual(result[0], acutal[0]);
            Assert.AreEqual(result[1], acutal[1]);
            Assert.AreEqual(result[2], acutal[2]);

        }


    }
}
