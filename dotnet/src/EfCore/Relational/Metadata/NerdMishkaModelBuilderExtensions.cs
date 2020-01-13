using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Humanizer;
using System;
using System.Linq.Expressions;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using NerdyMishka.EfCore.Metadata;

namespace NerdyMishka.EfCore
{
    public static class NerdyMishkaModelBuilderExtensions
    {

        public static ModelBuilder ApplyNerdyMishkaNamingConventions(this ModelBuilder builder)
        {
            var conventions = new List<IEfCoreConvention>();
            conventions.AddRange(new NerdyMishkaConstraintConventions().Conventions);
            
            builder.ApplyNerdyMishkaNamingConventions(conventions);
            return builder;
        }
        
        
        public static ModelBuilder ApplyNerdyMishkaNamingConventions(
            this ModelBuilder builder, IEnumerable<IEfCoreConvention> conventions)
        {
            if(conventions == null || conventions.Count() == 0)
                return builder;


            var entityTypeConventions = new List<IEntityTypeConvention>();
            var relationalEntityTypeConventions = new List<IRelationalEntityTypeConvention>(); 

            var propertyConventions = new List<IPropertyConvention>();
            var relationalPropertyConventions = new List<IRelationalPropertyConvention>(); 

            var indexConventions = new List<IIndexConvention>();
            var relationalIndexConventions = new List<IRelationalIndexConvention>();

            var relationalKeyConventions = new List<IRelationalKeyConvention>();
            var keyConventions = new List<IKeyConvention>();
            
            var fkConventions = new List<IForeignKeyConvention>();
            var relationalFkConventions = new List<IRelationalForeignKeyConvention>();

            var navConventions = new List<INavigationConvention>();
            var sequenceConventions = new List<ISequenceConvention>();

            

            foreach(var c in conventions)
            {
                switch(c)
                {
                    case IEntityTypeConvention et:
                        entityTypeConventions.Add(et);
                        break;

                    case IRelationalEntityTypeConvention ret:
                        relationalEntityTypeConventions.Add(ret);
                        break;
                    
                    case IPropertyConvention pc:
                        propertyConventions.Add(pc);
                        break;
                    
                    case IRelationalPropertyConvention rpc:
                        relationalPropertyConventions.Add(rpc);
                        break;

                    case IKeyConvention kc:
                        keyConventions.Add(kc);
                        break;

                    case IRelationalKeyConvention rkc:
                        relationalKeyConventions.Add(rkc);
                        break;

                    case IIndexConvention ic:
                        indexConventions.Add(ic);
                        break;

                    case IRelationalIndexConvention irc:
                        relationalIndexConventions.Add(irc);
                        break;

                    case IForeignKeyConvention fkc:
                        fkConventions.Add(fkc);
                        break;

                    case IRelationalForeignKeyConvention rfkc:
                        relationalFkConventions.Add(rfkc);
                        break;

                    case ISequenceConvention sc:
                        sequenceConventions.Add(sc);
                        break;

                    case INavigationConvention nc:
                        navConventions.Add(nc);
                        break;
                }
            }

            var entityTypes = builder.Model.GetEntityTypes();


            foreach (var entityType in entityTypes)
            {
                if(entityTypeConventions.Count > 0) {
                    foreach(var c in entityTypeConventions)
                        c.Apply(entityType);
                }

                if(relationalEntityTypeConventions.Count > 0) {
                    var annotations = entityType;
                    foreach(var c in relationalEntityTypeConventions)
                        c.Apply(annotations);
                }

                if(propertyConventions.Count > 0 || relationalPropertyConventions.Count > 0)
                {
                    
                    foreach(var property in entityType.GetProperties())
                    {
                      
                        if(propertyConventions.Count > 0) {
                            foreach(var c in propertyConventions) 
                                c.Apply(property);
                        }

                        if(relationalEntityTypeConventions.Count > 0) {
                            var annotation = property;
                            foreach(var c in relationalPropertyConventions)
                                c.Apply(annotation);
                        }
                    }
                }

                if(keyConventions.Count > 0 || relationalKeyConventions.Count > 0)
                {
                    foreach(var key in entityType.GetKeys())
                    {
                        
                        if(keyConventions.Count > 0)
                        {
                            foreach(var c in keyConventions)
                                c.Apply(key);
                        }

                        if(relationalKeyConventions.Count > 0)
                        {
                            var annotation = key;
                            foreach(var c in relationalKeyConventions)
                                c.Apply(annotation);
                        }
                    }
                }

                if(indexConventions.Count > 0 || relationalIndexConventions.Count > 0) {
                    
                    foreach(var ix in entityType.GetIndexes())
                    {
                        
                        if(indexConventions.Count > 0)
                        {
                            foreach(var c in indexConventions)
                                c.Apply(ix);
                        }

                        if(relationalIndexConventions.Count > 0)
                        {
                            var annotation = ix;
                            foreach(var c in relationalIndexConventions)
                                c.Apply(annotation);
                        }
                    }
                }
            }

            // make a second loop so that all entity types names and properties 
            // are updated first from a naming perspective.  
            foreach(var entityType in entityTypes)
            {
                if(fkConventions.Count > 0 || relationalFkConventions.Count > 0)
                {
                    var fks = entityType.GetForeignKeys();
                    foreach(var fk in fks)
                    {
                        if(fkConventions.Count > 0)
                        {
                            foreach(var c in fkConventions)
                                c.Apply(fk);
                        }

                        if(relationalFkConventions.Count > 0)
                        {
                            var annotation = fk;
                            foreach(var c in relationalFkConventions)
                                c.Apply(annotation);
                        }
                    }
                }
                        
                   
                if(navConventions.Count > 0)
                {
                    var navigations = entityType.GetNavigations();
                    foreach(var nav in navigations)
                    {
                        foreach(var c in navConventions)
                            c.Apply(nav);
                    }
                }
            }

           
            var sequences =  builder.Model.GetSequences();
            if(sequences != null && sequences.Count > 0)
            {
                foreach(var s in sequences)
                {
                    foreach(var c in sequenceConventions)
                        c.Apply(s);
                }
            }   

            return builder;
        }
    }
}