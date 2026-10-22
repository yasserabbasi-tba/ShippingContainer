namespace Container;

public class ContainerNumber
{
    private readonly string _owner;
    private readonly string _equipmentCategory;
    private readonly string _serialNumber;
    private readonly byte _checkDigit;
    private readonly Dictionary<char, byte> _characterWeights = InitializeWeights();

    private static Dictionary<char, byte> InitializeWeights()
    {
        var dict = new Dictionary<char, byte>();

        for (char c = '0'; c <= '9'; c++)
        {
            dict[c] = (byte)(c - '0');
        }

        byte val = 10;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (val % 11 == 0) val++;
            dict[c] = val++;
        }

        return dict;
    }


    public override string ToString()
    {
        return $"{_owner}{_equipmentCategory}{_serialNumber}{_checkDigit}";
    }

    public ContainerNumber(string containerNumber)
    {
        string upperContainerNumber =
        ValidateLength(containerNumber);
        ValidateEquipmentCategory(upperContainerNumber);
        ValidateSerialNumberDigits(upperContainerNumber);

        _owner = upperContainerNumber[..3];
        _equipmentCategory = upperContainerNumber[3].ToString();
        _serialNumber = upperContainerNumber.Substring(4, 6);
        _checkDigit = ValidateAndExtractCheckDigit(upperContainerNumber);
    }

    private static string ValidateLength(string containerNumber)
    {
        if (string.IsNullOrEmpty(containerNumber) || containerNumber.Length < 11)
        {
            throw new ArgumentException("Argument is too short");
        }
        return containerNumber.ToUpper();
    }

    private static void ValidateEquipmentCategory(string containerNumber)
    {
        char category = containerNumber[3];
        if (category != 'U' && category != 'J' && category != 'Z')
        {
            throw new ArgumentException("4th character is not a valid equipment category group (U, J, or Z)");
        }
    }

    private static void ValidateSerialNumberDigits(string containerNumber)
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
        byte expectedCheckDigit = CalculateCheckDigit(containerNumber[..10]);

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