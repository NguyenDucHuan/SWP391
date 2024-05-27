﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using ProjectDiamondShop.Models;

namespace ProjectDiamondShop.Repositories
{
    public class DiamondRepository
    {
        private string connectionString;

        public DiamondRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<Diamond> GetDiamondList()
        {
            List<Diamond> diamonds = new List<Diamond>();
            try
            {
                using (SqlConnection coon = new SqlConnection(connectionString))
                {
                    coon.Open();
                    String getAllQuerry = "SELECT * FROM [dbo].[tblDiamonds]";
                    using (SqlCommand command = new SqlCommand(getAllQuerry,coon))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double weight = reader.GetDouble(reader.GetOrdinal("caratWeight"));
                                Diamond diamond = new Diamond
                                {
                                    diamondID = reader.GetInt32(reader.GetOrdinal("diamondID")),
                                    diamondName = reader.GetString(reader.GetOrdinal("diamondName")),
                                    diamondPrice = reader.GetDecimal(reader.GetOrdinal("diamondPrice")),
                                    diamondDescription = reader.GetString(reader.GetOrdinal("diamondDescription")),
                                    caratWeight =(float)reader.GetDouble(reader.GetOrdinal("caratWeight")),
                                    clarityID = reader.GetString(reader.GetOrdinal("clarityID")),
                                    cutID = reader.GetString(reader.GetOrdinal("cutID")),
                                    colorID = reader.GetString(reader.GetOrdinal("colorID")),
                                    shapeID = reader.GetString(reader.GetOrdinal("shapeID")),
                                    diamondImagePath = reader.GetString(reader.GetOrdinal("diamondImagePath")),
                                    status = false
                                };
                                diamonds.Add(diamond);
                            }
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return diamonds;
        }

        public List<Diamond> GetFilteredDiamonds(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight)
        {
            List<Diamond> diamonds = new List<Diamond>();
            try 
            {
                using (SqlConnection coon = new SqlConnection(connectionString))
                {
                    coon.Open();
                    String query = "SELECT * FROM [dbo].[tblDiamonds] WHERE 1=1";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " AND diamondName LIKE @searchTerm";
                    }
                    if (!string.IsNullOrEmpty(clarity))
                    {
                        query += " AND clarityID = @clarity";
                    }
                    if (!string.IsNullOrEmpty(cut))
                    {
                        query += " AND cutID = @cut";
                    }
                    if (!string.IsNullOrEmpty(color))
                    {
                        query += " AND colorID = @color";
                    }
                    if (!string.IsNullOrEmpty(shape))
                    {
                        query += " AND shapeID = @shape";
                    }
                    if (minPrice.HasValue)
                    {
                        query += " AND diamondPrice >= @minPrice";
                    }
                    if (maxPrice.HasValue)
                    {
                        query += " AND diamondPrice <= @maxPrice";
                    }
                    if (minCaratWeight.HasValue)
                    {
                        query += " AND caratWeight >= @minCaratWeight";
                    }
                    if (maxCaratWeight.HasValue)
                    {
                        query += " AND caratWeight <= @maxCaratWeight";
                    }
                    using (SqlCommand command = new SqlCommand(query, coon))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }
                        if (!string.IsNullOrEmpty(clarity))
                        {
                            command.Parameters.AddWithValue("@clarity", clarity);
                        }
                        if (!string.IsNullOrEmpty(cut))
                        {
                            command.Parameters.AddWithValue("@cut", cut);
                        }
                        if (!string.IsNullOrEmpty(color))
                        {
                            command.Parameters.AddWithValue("@color", color);
                        }
                        if (!string.IsNullOrEmpty(shape))
                        {
                            command.Parameters.AddWithValue("@shape", shape);
                        }
                        if (minPrice.HasValue)
                        {
                            command.Parameters.AddWithValue("@minPrice", minPrice.Value);
                        }
                        if (maxPrice.HasValue)
                        {
                            command.Parameters.AddWithValue("@maxPrice", maxPrice.Value);
                        }
                        if (minCaratWeight.HasValue)
                        {
                            command.Parameters.AddWithValue("@minCaratWeight", minCaratWeight.Value);
                        }
                        if (maxCaratWeight.HasValue)
                        {
                            command.Parameters.AddWithValue("@maxCaratWeight", maxCaratWeight.Value);
                        }
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) {
                                double weight = reader.GetDouble(reader.GetOrdinal("caratWeight"));
                                Diamond diamond = new Diamond
                                {
                                    diamondID = reader.GetInt32(reader.GetOrdinal("diamondID")),
                                    diamondName = reader.GetString(reader.GetOrdinal("diamondName")),
                                    diamondPrice = reader.GetDecimal(reader.GetOrdinal("diamondPrice")),
                                    diamondDescription = reader.GetString(reader.GetOrdinal("diamondDescription")),
                                    caratWeight = (float)reader.GetDouble(reader.GetOrdinal("caratWeight")),
                                    clarityID = reader.GetString(reader.GetOrdinal("clarityID")),
                                    cutID = reader.GetString(reader.GetOrdinal("cutID")),
                                    colorID = reader.GetString(reader.GetOrdinal("colorID")),
                                    shapeID = reader.GetString(reader.GetOrdinal("shapeID")),
                                    diamondImagePath = reader.GetString(reader.GetOrdinal("diamondImagePath")),
                                    status = false
                                };
                                diamonds.Add(diamond);
                            }
                        }
                    }
                }
            }catch (SqlException ex) 
            { 
                Console.WriteLine(ex.Message); 
            }
            return diamonds;
        }
        }
}