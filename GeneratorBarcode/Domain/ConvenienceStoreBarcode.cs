﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorBarcode.Domain
{
    public class ConvenienceStoreBarcode
    {
        public string OrderSeq { get; set; }
        public string SuffixCode { get; set; }
        public decimal Premium { get; set; }
        public List<int[]> DraftInt { get; set; } = new List<int[]>();
        public List<string[]> DraftString { get; set; } = new List<string[]>();

        public int[] CheckCode 
        {
            get { return CreateCheckCode();}
        }
        public DateTime PaymentDeadline { get; set; }

        public String PaymentDeadlineChinese
        {
            get { return TranYearToChineseYear(PaymentDeadline); }
        }

        public ConvenienceStoreBarcode(){}

        /// <summary>
        /// 超商條碼
        /// </summary>
        /// <param name="order_seq">訂單號碼</param>
        /// <param name="suffix_code">後置碼三碼(I0F、I01)</param>
        /// <param name="payment_deadline">繳費截止日</param>
        /// <param name="premium">保費</param>
        public ConvenienceStoreBarcode(string order_seq,string suffix_code, DateTime payment_deadline,decimal premium)
        {
            OrderSeq = order_seq;
            SuffixCode = suffix_code;
            PaymentDeadline = payment_deadline;
            Premium = premium;
            CreateDraft();
        }

        /// <summary>
        /// 超商條碼
        /// </summary>
        /// <param name="order_seq">訂單號碼</param>
        /// <param name="suffix_code">後置碼三碼(I0F、I01)</param>
        /// <param name="payment_deadline">繳費截止日</param>
        /// <param name="premium">保費</param>
        public string[] GetBarcode(string order_seq, string suffix_code, DateTime payment_deadline, decimal premium)
        {
            OrderSeq = order_seq;
            SuffixCode = suffix_code;
            PaymentDeadline = payment_deadline;
            Premium = premium;
            CreateDraft();
            return GetBarcode();
        }

        public string[] GetBarcode()
        {
            string[] result = { "", "", "" };
            int i = 0;
            foreach(var item in DraftString)
            {
                //第三個條碼要帶入檢查碼
                if (i == 2)
                {
                    item[4] = CheckCode[0] == 0 ? "A" : CheckCode[0] == 10 ? "B" : CheckCode[0].ToString();
                    item[5] = CheckCode[1] == 0 ? "X" : CheckCode[1] == 10 ? "Y" : CheckCode[1].ToString();    
                }
                result[i] = string.Join("", item);
                i++;
            }
            return result;
        }

        public int[] CreateCheckCode()
        {
            int[] checkCode = {0,0};
            checkCode[0]= GetEvenOrOddSum(0)%11;
            checkCode[1] = GetEvenOrOddSum(1)%11;
            return checkCode;
        }

        private int GetEvenOrOddSum(int index)
        {
            int total = 0;
            foreach (var item in DraftInt)
            {
                for (int i = index; i < item.Length; i = i + 2)
                {
                    total += item[i];
                }
            }
            return total;
        }

        public void CreateDraft()
        {
            Tuple<int[], string[]> tuple;
            tuple = GeneratorOne();
            DraftInt.Add(tuple.Item1);
            DraftString.Add(tuple.Item2);

            tuple = GeneratorTwo();
            DraftInt.Add(tuple.Item1);
            DraftString.Add(tuple.Item2);

            tuple = GeneratorThree();
            DraftInt.Add(tuple.Item1);
            DraftString.Add(tuple.Item2);
        }

        public Tuple<int[],string[]> GeneratorOne()
        {
            string temp = PaymentDeadlineChinese.Substring(2, 6) + SuffixCode;
            int[] result = temp.Select(x =>LetterToNumber(x)).ToArray();
            string[] resultS = temp.Select(x => x.ToString()).ToArray();
            Tuple<int[], string[]> tuple = new Tuple<int[], string[]>(result, resultS);
            return tuple;
        }

        public Tuple<int[], string[]> GeneratorTwo()
        {
            string temp = OrderSeq.Substring(0, 16);
            int[] result = temp.Select(x => LetterToNumber(x)).ToArray();
            string[] resultS = temp.Select(x => x.ToString()).ToArray();
            Tuple<int[], string[]> tuple = new Tuple<int[], string[]>(result, resultS);
            return tuple;
        }

        public Tuple<int[], string[]> GeneratorThree()
        {
            string temp = OrderSeq.Substring(16, 4) + "00" + Premium.ToString("000000000");
            int[] result = temp.Select(x => LetterToNumber(x)).ToArray();
            string[] resultS = temp.Select(x => x.ToString()).ToArray();
            Tuple<int[], string[]> tuple = new Tuple<int[], string[]>(result, resultS);
            return tuple;
        }

        private int LetterToNumber(char letter)
        {
            int result;
            var LetterMap = GetLetterMap();
            LetterMap.TryGetValue(letter, out result);
            if (result == 0) result = letter-48;
            return result;
        }

        private static Dictionary<char, int> GetLetterMap()
        {
            Dictionary<char, int> LetterMap = new Dictionary<char, int>();
            LetterMap.Add('A', 1);
            LetterMap.Add('B', 2);
            LetterMap.Add('C', 3);
            LetterMap.Add('D', 4);
            LetterMap.Add('E', 5);
            LetterMap.Add('F', 6);
            LetterMap.Add('G', 7);
            LetterMap.Add('H', 8);
            LetterMap.Add('I', 9);
            LetterMap.Add('J', 1);
            LetterMap.Add('K', 2);
            LetterMap.Add('L', 3);
            LetterMap.Add('M', 4);
            LetterMap.Add('N', 5);
            LetterMap.Add('O', 6);
            LetterMap.Add('P', 7);
            LetterMap.Add('Q', 8);
            LetterMap.Add('R', 9);
            LetterMap.Add('S', 2);
            LetterMap.Add('T', 3);
            LetterMap.Add('U', 4);
            LetterMap.Add('V', 5);
            LetterMap.Add('W', 6);
            LetterMap.Add('X', 7);
            LetterMap.Add('Y', 8);
            LetterMap.Add('Z', 9);

            return LetterMap;
        }

        public DateTime GetPaymentDeadline(DateTime create_datetime)
        {
            return create_datetime.AddDays(16);
        }

        public string TranYearToChineseYear(DateTime create_datetime)
        {
            var result = "";
            int year = create_datetime.Year;
            int month = create_datetime.Month;
            int day = create_datetime.Day;

            result = (year - 1911).ToString("0000") + month.ToString("00") + day.ToString("00");

            return result;
        }

    }

}
