namespace DotnetOrderService.Infrastructure.Dtos
{
    public class EnumDto {
        public int Key { get; set; }
        public string Value { get; set; }

        public EnumDto(int key, string value) {
            Key = key;
            Value = value;
        }
    }
}
