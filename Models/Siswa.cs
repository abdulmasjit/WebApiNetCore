namespace WebApiNetCore.Models;

using System.Text.Json.Serialization;

public class Siswa
{
    public int? Id { get; set; }
    public string? Nis { get; set; }
    public string? Nama { get; set; }
    public string? Alamat { get; set; }
    public string? Telepon { get; set; }
}