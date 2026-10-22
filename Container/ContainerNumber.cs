namespace Container;

public class ContainerNumber
{
    private readonly Dictionary<char, byte> _characterWeights = new()
    {
    {'A',10 },
    {'B',12 },
    {'C',13 },
    {'D',14 },
    {'E',15 },
    {'F',16 },
    {'G',17 },
    {'H',18 },
    {'I',19 },
    {'J',20 },
    {'K',21 },
    {'L',23 },
    {'M',24 },
    {'N',25 },
    {'O',26 },
    {'P',27 },
    {'Q',28 },
    {'R',29 },
    {'S',30 },
    {'T',31 },
    {'U',32 },
    {'V',34 },
    {'W',35 },
    {'X',36 },
    {'Y',37 },
    {'Z',38 },
    {'0',0 },
    {'1',1 },
    {'2',2 },
    {'3',3 },
    {'4',4 },
    {'5',5 },
    {'6',6 },
    {'7',7 },
    {'8',8 },
    {'9',9 }
    };

    private string _owner;
    private string _equipmentCategory;
    private string _serialNumber;
    private byte _checkDigit;

    public override string ToString()
    {
        return $"{_owner}{_equipmentCategory}{_serialNumber}{_checkDigit}";
    }

    public ContainerNumber(string containerNumber)
    {
        string upperContainerNumber = ValidateLength(containerNumber);
        ValidateEquipmentCategory(upperContainerNumber);
        ValidateSerialNumberDigits(upperContainerNumber);

        _owner = upperContainerNumber.Substring(0, 3);
        _equipmentCategory = upperContainerNumber[3].ToString();
        _serialNumber = upperContainerNumber.Substring(4, 6);
        _checkDigit = ValidateAndExtractCheckDigit(upperContainerNumber);
    }

    private string ValidateLength(string containerNumber)
    {
        if (string.IsNullOrEmpty(containerNumber) || containerNumber.Length < 11)
        {
            throw new ArgumentException("Argument is too short");
        }
        return containerNumber.ToUpper();
    }

    private void ValidateEquipmentCategory(string containerNumber)
    {
        char category = containerNumber[3];
        if (category != 'U' && category != 'J' && category != 'Z')
        {
            throw new ArgumentException("4th character is not a valid equipment category group (U, J, or Z)");
        }
    }

    private void ValidateSerialNumberDigits(string containerNumber)
    {
        for (int i = 4; i <= 9; i++)
        {
            if (!char.IsDigit(containerNumber[i]))
            {
                throw new ArgumentException("Characters 5-10 must be digits");
            }
        }
    }

    private byte ValidateAndExtractCheckDigit(string containerNumber)
    {
        byte expectedCheckDigit = CalculateCheckDigit(containerNumber.Substring(0, 10));

        char actualCheckDigitChar = containerNumber[10];
        if (!char.IsDigit(actualCheckDigitChar) || (byte)(actualCheckDigitChar - '0') != expectedCheckDigit)
        {
            throw new ArgumentException("Check digit is not correct");
        }

        return (byte)(actualCheckDigitChar - '0');
    }

    public byte CalculateCheckDigit(string first10Chars)
    {
        int totalSum = 0;

        for (int position = 0; position < 10; position++)
        {
            char character = first10Chars[position];

            if (!_characterWeights.TryGetValue(character, out byte numericalValue))
            {
                throw new ArgumentException($"Invalid character '{character}' at position {position + 1}.");
            }

            int factor = 1 << position;
            totalSum += numericalValue * factor;
        }

        int remainder = totalSum % 11;

        return (byte)(remainder == 10 ? 0 : remainder);
    }
}