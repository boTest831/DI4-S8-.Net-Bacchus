﻿using Bacchus.Utils;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bacchus.Model;

namespace Bacchus.DAO
{
    public class ArticleDAO
    {
        SQLiteConnection Connection;

        SousFamilleDAO SousFamilleDao;

        MarqueDAO MarqueDao;

        public ArticleDAO()
        {
            this.Connection = ConnectionDB.GetConnection();
            this.SousFamilleDao = new SousFamilleDAO();
            this.MarqueDao = new MarqueDAO();
        }


        public int Insert(Article Article)
        {
            int Count = 0;
            SQLiteCommand InsertCommand = new SQLiteCommand(Connection);
            using (SQLiteTransaction Transaction = Connection.BeginTransaction())
            {
                InsertCommand.CommandText = "INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT" +
                    ", Quantite) VALUES (:RefArticle, :Description, :RefSousFamille, :RefMarque, :PrixHT, :quantite)";
                InsertCommand.Parameters.AddWithValue("RefArticle", Article.Ref_Article);
                InsertCommand.Parameters.AddWithValue("Description", Article.Description);
                InsertCommand.Parameters.AddWithValue("RefSousFamille", Article.SousFamille.Ref_SousFamille);
                InsertCommand.Parameters.AddWithValue("RefMarque", Article.Marque.Ref_Marque);
                InsertCommand.Parameters.AddWithValue("PrixHT", Article.PrixHT);
                InsertCommand.Parameters.AddWithValue("quantite", Article.Quantite);

                Count = InsertCommand.ExecuteNonQuery();
                Transaction.Commit();
                InsertCommand.Dispose();

            }
            return Count;

        }

        public void Update(Article Article)
        {
            var Command =new SQLiteCommand("UPDATE Articles SET Description = :descript, RefSousFamille = :refSousFamille, RefMarque = :refMarque, PrixHT = :prix, Quantite = :quantite WHERE RefArticle = :refArticle", Connection);
            Command.Parameters.AddWithValue("descript", Article.Description);
            Command.Parameters.AddWithValue("refSousFamille", Article.SousFamille.Ref_SousFamille);
            Command.Parameters.AddWithValue("refMarque", Article.Marque.Ref_Marque);
            Command.Parameters.AddWithValue("prix", Article.PrixHT);
            Command.Parameters.AddWithValue("quantite", Article.Quantite);
            Command.Parameters.AddWithValue("refArticle", Article.Ref_Article);
            Command.ExecuteNonQuery();
        }
        public void Delete(Article Article)
        {

            var Command = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = :refArticle", Connection);
            Command.Parameters.AddWithValue("refArticle", Article.Ref_Article);
            Command.ExecuteNonQuery();

        }

        public List<Article> GetAllArticles()
        {
            var Articles = new List<Article>();

            var Command = new SQLiteCommand("SELECT * FROM Articles", Connection);
            var Reader = Command.ExecuteReader();

            while (Reader.Read())
            {
                var RefArticle = Reader.GetString(0);
                var Description = Reader.GetString(1);
                var SousFamille = SousFamilleDao.GetSousFamilleByID(Reader.GetInt32(2));
                var Marque = MarqueDao.GetMarqueByID(Reader.GetInt32(3));
                var PrixHT = Reader.GetFloat(4);
                var Quantite = Reader.GetInt32(5);

                Articles.Add(new Article(RefArticle, Description, SousFamille, Marque, PrixHT, Quantite));
            }

            Reader.Close();

            return Articles;
        }

        public void DeleteAllArticles()
        {
            var Transaction = Connection.BeginTransaction();
            SQLiteCommand Command = new SQLiteCommand("DELETE FROM Articles", Connection);
            Command.ExecuteNonQuery();
            Transaction.Commit();
        }
    }
}


   