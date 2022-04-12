using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace WebApiNetCore.Controllers;
using WebApiNetCore.Models;
using MySql.Data.MySqlClient;

[ApiController]
[Route("[controller]")]
public class SiswaController : ControllerBase
{
    private readonly ILogger<SiswaController> _logger;
    private readonly IConfiguration configuration;

    public SiswaController(ILogger<SiswaController> logger, IConfiguration config)
    {
        _logger = logger;
        configuration = config;
    }

    [HttpGet(Name = "siswa")]
    public IEnumerable<Siswa> Get()
    {
        IList<Siswa> items = new List<Siswa>();
        // koneksi database
        MySqlConnection conn = new MySqlConnection{
            ConnectionString = configuration.GetConnectionString("WebApiDatabase")
        };
        conn.Open();

        // menyiapkan query
        MySqlCommand cmd = new MySqlCommand("SELECT * FROM siswa;", conn);

        // membaca data
        MySqlDataReader dataReader = cmd.ExecuteReader();
        while (dataReader.Read()){
            // menyimpan record ke object model
            Siswa item = new Siswa();
            item.Id = Convert.ToInt16(dataReader["id"]);
            item.Nis = Convert.ToString(dataReader["nis"]);
            item.Nama = Convert.ToString(dataReader["nama"]);
            item.Alamat = Convert.ToString(dataReader["alamat"]);
            item.Telepon = Convert.ToString(dataReader["telepon"]);
            // menyimpan object model ke collection
            items.Add(item);
        }
        dataReader.Close();
        conn.Close();

        return items;
    }

    [HttpPost]
    public IActionResult Create(Siswa siswa){
        if (!ModelState.IsValid)
            return BadRequest("Invalid data.");

        MySqlConnection conn = new MySqlConnection{
            ConnectionString = configuration.GetConnectionString("WebApiDatabase")
        };
        conn.Open();

        MySqlCommand command = conn.CreateCommand();
        command.CommandText = "INSERT INTO siswa (nis, nama, alamat, telepon) VALUES (?nis, ?nama, ?alamat, ?telepon)";
        command.Parameters.AddWithValue("?nis", siswa.Nis);
        command.Parameters.AddWithValue("?nama", siswa.Nama);
        command.Parameters.AddWithValue("?alamat", siswa.Alamat);
        command.Parameters.AddWithValue("?telepon", siswa.Telepon);
        command.ExecuteNonQuery();
        conn.Close();

        return Ok();
    }

    [HttpPut("{id}")]
    public JsonResult Put(int id, Siswa siswa)
    {
        string query = @"
                    update siswa set 
                    nis = @nis,
                    nama = @nama,
                    alamat = @alamat,
                    telepon = @telepon
                    where id=@id;
        ";

        DataTable table = new DataTable();
        string sqlDataSource = configuration.GetConnectionString("WebApiDatabase");
        MySqlDataReader myReader;
        using (MySqlConnection conn = new MySqlConnection(sqlDataSource))
        {
            conn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, conn))
            {
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@nis", siswa.Nis);
                myCommand.Parameters.AddWithValue("@nama", siswa.Nama);
                myCommand.Parameters.AddWithValue("@alamat", siswa.Alamat);
                myCommand.Parameters.AddWithValue("@telepon", siswa.Telepon);

                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                conn.Close();
            }
        }

        return new JsonResult("Updated Successfully");
    }

    [HttpDelete("{id}")]
    public JsonResult Delete(int id)
    {
        string query = @"
                    delete from siswa
                    where id=@id;
        ";

        DataTable table = new DataTable();
        string sqlDataSource = configuration.GetConnectionString("WebApiDatabase");
        MySqlDataReader myReader;
        using (MySqlConnection conn = new MySqlConnection(sqlDataSource))
        {
            conn.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, conn))
            {
                myCommand.Parameters.AddWithValue("@id", id);

                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                conn.Close();
            }
        }

        return new JsonResult("Deleted Successfully");
    }

}
