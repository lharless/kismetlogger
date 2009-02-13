
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

// Generated by MyGeneration Version # (1.3.0.3)

using System;
using System.Data;
using Finisar.SQLite;
using System.Collections;
using System.Collections.Specialized;

using MyGeneration.dOOdads;

namespace KismetLogger
{
	public abstract class _encchanges : SQLiteEntity
	{
		public _encchanges()
		{
			this.QuerySource = "encchanges";
			this.MappingName = "encchanges";

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
		public virtual bool LoadByPrimaryKey(string Bssid, string Date)
		{
			this.Where.Bssid.Value = Bssid;
this.Where.Date.Value = Date;
			
			return this.Query.Load();
		}
		
		
		#region Parameters
		protected class Parameters
		{
			
			public static SQLiteParameter Bssid
			{
				get
				{
					return new SQLiteParameter("@Bssid", DbType.String);

				}
			}
			
			public static SQLiteParameter Date
			{
				get
				{
					return new SQLiteParameter("@Date", DbType.String);

				}
			}
			
			public static SQLiteParameter Oldenc
			{
				get
				{
					return new SQLiteParameter("@Oldenc", DbType.String);

				}
			}
			
			public static SQLiteParameter Newenc
			{
				get
				{
					return new SQLiteParameter("@Newenc", DbType.String);

				}
			}
			
		}
		#endregion		
	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string Bssid = "bssid";
            public const string Date = "date";
            public const string Oldenc = "oldenc";
            public const string Newenc = "newenc";

			static public string ToPropertyName(string columnName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[Bssid] = _encchanges.PropertyNames.Bssid;
					ht[Date] = _encchanges.PropertyNames.Date;
					ht[Oldenc] = _encchanges.PropertyNames.Oldenc;
					ht[Newenc] = _encchanges.PropertyNames.Newenc;

				}
				return (string)ht[columnName];
			}

			static private Hashtable ht = null;			 
		}
		#endregion
		
		#region PropertyNames
		public class PropertyNames
		{  
            public const string Bssid = "Bssid";
            public const string Date = "Date";
            public const string Oldenc = "Oldenc";
            public const string Newenc = "Newenc";

			static public string ToColumnName(string propertyName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[Bssid] = _encchanges.ColumnNames.Bssid;
					ht[Date] = _encchanges.ColumnNames.Date;
					ht[Oldenc] = _encchanges.ColumnNames.Oldenc;
					ht[Newenc] = _encchanges.ColumnNames.Newenc;

				}
				return (string)ht[propertyName];
			}

			static private Hashtable ht = null;			 
		}			 
		#endregion	

		#region StringPropertyNames
		public class StringPropertyNames
		{  
            public const string Bssid = "s_Bssid";
            public const string Date = "s_Date";
            public const string Oldenc = "s_Oldenc";
            public const string Newenc = "s_Newenc";

		}
		#endregion		
		
		#region Properties
	
		public virtual string Bssid
	    {
			get
	        {
				return base.Getstring(ColumnNames.Bssid);
			}
			set
	        {
				base.Setstring(ColumnNames.Bssid, value);
			}
		}

		public virtual string Date
	    {
			get
	        {
				return base.Getstring(ColumnNames.Date);
			}
			set
	        {
				base.Setstring(ColumnNames.Date, value);
			}
		}

		public virtual string Oldenc
	    {
			get
	        {
				return base.Getstring(ColumnNames.Oldenc);
			}
			set
	        {
				base.Setstring(ColumnNames.Oldenc, value);
			}
		}

		public virtual string Newenc
	    {
			get
	        {
				return base.Getstring(ColumnNames.Newenc);
			}
			set
	        {
				base.Setstring(ColumnNames.Newenc, value);
			}
		}


		#endregion
		
		#region String Properties
	
		public virtual string s_Bssid
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Bssid) ? string.Empty : base.GetstringAsString(ColumnNames.Bssid);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Bssid);
				else
					this.Bssid = base.SetstringAsString(ColumnNames.Bssid, value);
			}
		}

		public virtual string s_Date
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Date) ? string.Empty : base.GetstringAsString(ColumnNames.Date);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Date);
				else
					this.Date = base.SetstringAsString(ColumnNames.Date, value);
			}
		}

		public virtual string s_Oldenc
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Oldenc) ? string.Empty : base.GetstringAsString(ColumnNames.Oldenc);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Oldenc);
				else
					this.Oldenc = base.SetstringAsString(ColumnNames.Oldenc, value);
			}
		}

		public virtual string s_Newenc
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Newenc) ? string.Empty : base.GetstringAsString(ColumnNames.Newenc);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Newenc);
				else
					this.Newenc = base.SetstringAsString(ColumnNames.Newenc, value);
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
				
				
				public WhereParameter Bssid
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Bssid, Parameters.Bssid);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter Date
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Date, Parameters.Date);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter Oldenc
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Oldenc, Parameters.Oldenc);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter Newenc
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Newenc, Parameters.Newenc);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}


				private WhereClause _clause;
			}
			#endregion
		
			public WhereParameter Bssid
		    {
				get
		        {
					if(_Bssid_W == null)
	        	    {
						_Bssid_W = TearOff.Bssid;
					}
					return _Bssid_W;
				}
			}

			public WhereParameter Date
		    {
				get
		        {
					if(_Date_W == null)
	        	    {
						_Date_W = TearOff.Date;
					}
					return _Date_W;
				}
			}

			public WhereParameter Oldenc
		    {
				get
		        {
					if(_Oldenc_W == null)
	        	    {
						_Oldenc_W = TearOff.Oldenc;
					}
					return _Oldenc_W;
				}
			}

			public WhereParameter Newenc
		    {
				get
		        {
					if(_Newenc_W == null)
	        	    {
						_Newenc_W = TearOff.Newenc;
					}
					return _Newenc_W;
				}
			}

			private WhereParameter _Bssid_W = null;
			private WhereParameter _Date_W = null;
			private WhereParameter _Oldenc_W = null;
			private WhereParameter _Newenc_W = null;

			public void WhereClauseReset()
			{
				_Bssid_W = null;
				_Date_W = null;
				_Oldenc_W = null;
				_Newenc_W = null;

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
				
				
				public AggregateParameter Bssid
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Bssid, Parameters.Bssid);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter Date
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Date, Parameters.Date);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter Oldenc
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Oldenc, Parameters.Oldenc);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter Newenc
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Newenc, Parameters.Newenc);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}


				private AggregateClause _clause;
			}
			#endregion
		
			public AggregateParameter Bssid
		    {
				get
		        {
					if(_Bssid_W == null)
	        	    {
						_Bssid_W = TearOff.Bssid;
					}
					return _Bssid_W;
				}
			}

			public AggregateParameter Date
		    {
				get
		        {
					if(_Date_W == null)
	        	    {
						_Date_W = TearOff.Date;
					}
					return _Date_W;
				}
			}

			public AggregateParameter Oldenc
		    {
				get
		        {
					if(_Oldenc_W == null)
	        	    {
						_Oldenc_W = TearOff.Oldenc;
					}
					return _Oldenc_W;
				}
			}

			public AggregateParameter Newenc
		    {
				get
		        {
					if(_Newenc_W == null)
	        	    {
						_Newenc_W = TearOff.Newenc;
					}
					return _Newenc_W;
				}
			}

			private AggregateParameter _Bssid_W = null;
			private AggregateParameter _Date_W = null;
			private AggregateParameter _Oldenc_W = null;
			private AggregateParameter _Newenc_W = null;

			public void AggregateClauseReset()
			{
				_Bssid_W = null;
				_Date_W = null;
				_Oldenc_W = null;
				_Newenc_W = null;

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
			@"INSERT INTO [encchanges]
			(
				[bssid],
				[date],
				[oldenc],
				[newenc]
			)
			VALUES
			(
				@bssid,
				@date,
				@oldenc,
				@newenc
			)";

			CreateParameters(cmd);
			return cmd;
		}
	
		protected override IDbCommand GetUpdateCommand()
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
			@"UPDATE [encchanges] SET 
				[oldenc]=@oldenc,
				[newenc]=@newenc
			WHERE
				[bssid]=@bssid AND 
				[date]=@date";

			CreateParameters(cmd);
			return cmd;
		}
	
		protected override IDbCommand GetDeleteCommand()
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
			@"DELETE FROM [encchanges] 
			WHERE
				[bssid]=@bssid AND 
				[date]=@date";

	
			SQLiteParameter p;
			p = cmd.Parameters.Add(Parameters.Bssid);
			p.SourceColumn = ColumnNames.Bssid;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Date);
			p.SourceColumn = ColumnNames.Date;
			p.SourceVersion = DataRowVersion.Current;

  
			return cmd;
		}
		
		private IDbCommand CreateParameters(SQLiteCommand cmd)
		{
			SQLiteParameter p;
		
			p = cmd.Parameters.Add(Parameters.Bssid);
			p.SourceColumn = ColumnNames.Bssid;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Date);
			p.SourceColumn = ColumnNames.Date;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Oldenc);
			p.SourceColumn = ColumnNames.Oldenc;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Newenc);
			p.SourceColumn = ColumnNames.Newenc;
			p.SourceVersion = DataRowVersion.Current;


			return cmd;
		}		
		
	
	}
}