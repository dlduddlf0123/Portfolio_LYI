using UnityEngine;

public class ChangeMoneyUnit {

    //돈 단위 저장 배열
    char[] moneyUnit;

    ChangeMoneyUnit()
    {
        moneyUnit = new char[]{
            'k', 'm', 'b', 't','q','s',
            'A', 'B', 'C', 'D', 'E',
            'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O',
            'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z' };

    }
    
    //돈 값을 받아서 1000단위마다 잘라 단위를 변경하는 함수
    public string PrintMoney(int nMoney)
    { 
        //1000아래면 바로 리턴(단위 변환 x)
        if (nMoney < 1000)
            return nMoney.ToString();

        //정수를 문자열로 변환
        string sMoney = nMoney.ToString();

        //1,10,100단위 구분
        switch ((sMoney.Length-1) % 3)
        {
            case 0:
                return string.Format("{0:#.##}{1}", (float)nMoney / Mathf.Pow(10, sMoney.Length - 1), moneyUnit[(sMoney.Length / 3) - 1]);
            case 1:
                return string.Format("{0:#.##}{1}", (float)nMoney / Mathf.Pow(10, sMoney.Length - 2), moneyUnit[(sMoney.Length / 3) - 1]);
            case 2:
                return string.Format("{0:#.##}{1}", (float)nMoney / Mathf.Pow(10, sMoney.Length - 3), moneyUnit[(sMoney.Length / 3) - 2]);
        }

        return nMoney.ToString();
    }

    /*string ChangeMoney(string haveGold)
    {
        string[] unit = new string[] { "", "A", "B", "C", "D", "E", "F", "G", "H", "I" };
        int[] cVal = new int[10];

        int index = 0;

        while (true)
        {
            string last4 = "";
            if (haveGold.Length >= 4)
            {
                last4 = haveGold.Substring(haveGold.Length - 4);
                int intLast4 = int.Parse(last4);

                cVal[index] = intLast4 % 1000;

                haveGold = haveGold.Remove(haveGold.Length - 3);
            }
            else
            {
                cVal[index] = int.Parse(haveGold);
                break;
            }

            index++;
        }

        if (index > 0)
        {
            int r = cVal[index] * 1000 + cVal[index - 1];
            return string.Format("{0:#.#}{1}", (float)r / 1000f, unit[index]);
        }

        return haveGold;
    }*/
}
