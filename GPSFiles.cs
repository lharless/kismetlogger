
/*
//===============================================================================
//  Generated From - SQLite_CSharp_BusinessEntity.vbgen
//
//  The supporting base class SQLiteEntity is in the Architecture directory in "dOOdads".
//  
//  This object is 'abstract' which means you need to inherit from it to be able
//  to instantiate it.  This is very easilly done. You can override properties and
//  methods in your derived class, this allows you to regenerate this class at any
//  time and not worry about overwriting custom code. 
//
//  NEVER EDIT THIS FILE.
//
//  public class YourObject :  _YourObject
//  {
//
//  }
//
//===============================================================================
*/

// Generated by MyGeneration Version # (1.2.0.2)

using System;
using System.Data;
using Finisar.SQLite;
using System.Collections;
using System.Collections.Specialized;

using MyGeneration.dOOdads;

namespace KismetLogger
{
	public abstract class _GPSFiles : SQLiteEntity
	{
		public _GPSFiles()
		{
			this.QuerySource = "GPSFiles";
			this.MappingName = "GPSFiles";

		}	

		//=================================================================
		//  public Overrides void AddNew()
		//=================================================================
		//
		//=================================================================
		public override void AddNew()
		{
			base.AddNew();
		}
		
		public override void FlushData()
		{
			this._whereClause = null;
			this._aggregateClause = null;
			base.FlushData();
		}
		
		public override string GetAutoKeyColumns()
		{
			return "";
		}
		

		//=================================================================
		//  	public Function LoadAll() As Boolean
		//=================================================================
		//  Loads all of the records in the database, and sets the currentRow to the first row
		//=================================================================
		public bool LoadAll() 
		{
			return this.Query.Load();
		}
	
	
		//=================================================================
		// public Overridable Function LoadByPrimaryKey()  As Boolean
		//=================================================================
		//  Loads a single row of via the primary key
		//=================================================================
		public virtual bool LoadByPrimaryKey(string Filename)
		{
			this.Where.Filename.Value = Filename;
			
			return this.Query.Load();
		}
		
		
		#region Parameters
		protected class Parameters
		{
			
			public static SQLiteParameter Filename
			{
				get
				{
					return new SQLiteParameter("@Filename", DbType.String);

				}
			}
			
		}
		#endregion		
	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string Filename = "filename";

			static public string ToPropertyName(string columnName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[Filename] = _GPSFiles.PropertyNames.Filename;

				}
				return (string)ht[columnName];
			}

			static private Hashtable ht = null;			 
		}
		#endregion
		
		#region PropertyNames
		public class PropertyNames
		{  
            public const string Filename = "Filename";

			static public string ToColumnName(string propertyName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[Filename] = _GPSFiles.ColumnNames.Filename;

				}
				return (string)ht[propertyName];
			}

			static private Hashtable ht = null;			 
		}			 
		#endregion	

		#region StringPropertyNames
		public class StringPropertyNames
		{  
            public const string Filename = "s_Filename";

		}
		#endregion		
		
		#region Properties
	
		public virtual string Filename
	    {
			get
	        {
				return base.Getstring(ColumnNames.Filename);
			}
			set
	        {
				base.Setstring(ColumnNames.Filename, value);
			}
		}


		#endregion
		
		#region String Properties
	
		public virtual string s_Filename
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Filename) ? string.Empty : base.GetstringAsString(ColumnNames.Filename);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Filename);
				else
					this.Filename = base.SetstringAsString(ColumnNames.Filename, value);
			}
		}


		#endregion		
	
		#region Where Clause
		public class WhereClause
		{
			public WhereClause(BusinessEntity entity)
			{
				this._entity = entity;
			}
			
			public TearOffWhereParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffWhereParameter(this);
					}

					return _tearOff;
				}
			}

			#region WhereParameter TearOff's
			public class TearOffWhereParameter
			{
				public TearOffWhereParameter(WhereClause clause)
				{
					this._clause = clause;
				}
				
				
				public WhereParameter Filename
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Filename, Parameters.Filename);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}


				private WhereClause _clause;
			}
			#endregion
		
			public WhereParameter Filename
		    {
				get
		        {
					if(_Filename_W == null)
	        	    {
						_Filename_W = TearOff.Filename;
					}
					return _Filename_W;
				}
			}

			private WhereParameter _Filename_W = null;

			public void WhereClauseReset()
			{
				_Filename_W = null;

				this._entity.Query.FlushWhereParameters();

			}
	
			private BusinessEntity _entity;
			private TearOffWhereParameter _tearOff;
			
		}
	
		public WhereClause Where
		{
			get
			{
				if(_whereClause == null)
				{
					_whereClause = new WhereClause(this);
				}
		
				return _whereClause;
			}
		}
		
		private WhereClause _whereClause = null;	
		#endregion
		
		#region Aggregate Clause
		public class AggregateClause
		{
			public AggregateClause(BusinessEntity entity)
			{
				this._entity = entity;
			}
			
			public TearOffAggregateParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffAggregateParameter(this);
					}

					return _tearOff;
				}
			}

			#region AggregateParameter TearOff's
			public class TearOffAggregateParameter
			{
				public TearOffAggregateParameter(AggregateClause clause)
				{
					this._clause = clause;
				}
				
				
				public AggregateParameter Filename
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Filename, Parameters.Filename);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}


				private AggregateClause _clause;
			}
			#endregion
		
			public AggregateParameter Filename
		    {
				get
		        {
					if(_Filename_W == null)
	        	    {
						_Filename_W = TearOff.Filename;
					}
					return _Filename_W;
				}
			}

			private AggregateParameter _Filename_W = null;

			public void AggregateClauseReset()
			{
				_Filename_W = null;

				this._entity.Query.FlushAggregateParameters();

			}
	
			private BusinessEntity _entity;
			private TearOffAggregateParameter _tearOff;
			
		}
	
		public AggregateClause Aggregate
		{
			get
			{
				if(_aggregateClause == null)
				{
					_aggregateClause = new AggregateClause(this);
				}
		
				return _aggregateClause;
			}
		}
		
		private AggregateClause _aggregateClause = null;	
		#endregion
	
			
		
		protected override IDbCommand GetInsertCommand()
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
			@"INSERT INTO [GPSFiles]
			(
				[filename]
			)
			VALUES
			(
				@filename
			)";

			CreateParameters(cmd);
			return cmd;
		}
	
		protected override IDbCommand GetUpdateCommand()
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
			@"UPDATE [GPSFiles] SET 

			WHERE
				[filename]=@filename";

			CreateParameters(cmd);
			return cmd;
		}
	
		protected override IDbCommand GetDeleteCommand()
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
			@"DELETE FROM [GPSFiles] 
			WHERE
				[filename]=@filename";

	
			SQLiteParameter p;
			p = cmd.Parameters.Add(Parameters.Filename);
			p.SourceColumn = ColumnNames.Filename;
			p.SourceVersion = DataRowVersion.Current;

  
			return cmd;
		}
		
		private IDbCommand CreateParameters(SQLiteCommand cmd)
		{
			SQLiteParameter p;
		
			p = cmd.Parameters.Add(Parameters.Filename);
			p.SourceColumn = ColumnNames.Filename;
			p.SourceVersion = DataRowVersion.Current;


			return cmd;
		}		
		
	
	}
}
