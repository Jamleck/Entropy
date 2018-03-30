using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PropertyComparison
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EqualsShouldReturnTrueIfAllValuesAreTrue()
        {
            var user1 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            var user2 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                //Dob = new DateTime(2000,1,1,12,0,0)
            };

            Assert.IsTrue(user1.Equals(user2));
        }


        public void EqualsShouldReturnFalseIfAnyPropertyIsNotEqual()
        {
            var user1 = new User()
            {
                Id = 1,
                FirstName = "Jane",
                SecondName = "Doe",
                //Dob = new DateTime(1988,1,1,12,0,0)
            };

            var user2 = new User()
            {
                Id = 2,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            Assert.IsTrue(user1.Equals(user2)); // Should not be equal
        }

        [TestMethod]
        public void EqualsOperatorShouldReturnTrueIfAllValuesAreEqual()
        {
            var user1 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            var user2 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };


            Assert.IsTrue(user1 == user2);
        }


        [TestMethod]
        public void NotEqualsOperatorShouldReturnFalseIfAllValuesAreEqual()
        {
            var user1 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            var user2 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            Assert.IsFalse(user1 != user2);
        }

        [TestMethod]
        public void EqualsOperatorShouldReturnFalseWhenFieldsAreNotEqual()
        {
            var user1 = new User()
            {
                Height = 11111

            };

            var user2 = new User()
            {
                Height = 99999
            };

            Assert.IsFalse(user1 == user2);
        }


        [TestMethod]
        public void EqualsOperatorShouldReturnTrueWhenFieldsAreNotEqual()
        {
            var user1 = new User()
            {
                Height = 111,
            };

            var user2 = new User()
            {
                Height = 111
            };

            Assert.IsTrue(user1 == user2);
        }



        [TestMethod]
        public void CanHandleNullObjectEquality()
        {
            User user1 = null;

            var user2 = new User()
            {
                Id = 1,
                FirstName = "John",
                SecondName = "Doe",
                Dob = new DateTime(2000, 1, 1, 12, 0, 0)
            };

            Assert.IsFalse(user1 == user2);
            Assert.IsTrue(user1 != user2);
            Assert.IsTrue(user1 == null);
            Assert.IsTrue(user1 == user1);
        }
    }

    public class Crew : PropertyObject
    {
        public int Id { get; set; } = 1;
    }

    public class User : PropertyObject
    {
        public int Height = 1;
        public int Id { get; set; } = 1;
        public string FirstName { get; set; } = "Test";
        public string SecondName { get; set; } = "Test";
        public DateTime Dob { get; set; } = new DateTime(2000, 1, 1, 12, 0, 0);
        List<Crew> Crews { get; set; }
    }

    public class PropertyObject : IEquatable<PropertyObject>
    {

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> Properties = new ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>>();
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<FieldInfo>> Fields = new ConcurrentDictionary<Type, IReadOnlyCollection<FieldInfo>>();

        public bool Equals(PropertyObject obj)
        {
            return Equals(obj as object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
            if (obj == null || GetType() != obj.GetType()) return false;

            var properties = GetProperties();
            var fields = GetFields();

            return properties.All(p => object.Equals(p.GetValue(this, null), p.GetValue(obj, null))) &&
            fields.All(f => object.Equals(f.GetValue(this), f.GetValue(obj)));
        }

        public static bool operator ==(PropertyObject obj1, PropertyObject obj2)
        {
            if (object.Equals(obj1, null))
            {
                if (object.Equals(obj2, null))
                {
                    return true;
                }
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(PropertyObject obj1, PropertyObject obj2)
        {
            return !(obj1 == obj2);
        }

        private IEnumerable<PropertyInfo> GetProperties()
        {

            return Properties.GetOrAdd(GetType(),
              t => t
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToList());
        }


        private IEnumerable<FieldInfo> GetFields()
        {

            return Fields.GetOrAdd(GetType(), t => t.GetFields(BindingFlags.Instance | BindingFlags.Public).ToList());

        }

        public override int GetHashCode()
        {
            unchecked   //allow overflow
            {
                int hash = 23;
                foreach (var prop in GetProperties())
                {
                    var value = prop.GetValue(this, null);
                    hash = HashValue(hash, value);
                }

                foreach (var field in GetFields())
                {
                    var value = field.GetValue(this);
                    hash = HashValue(hash, value);
                }

                return hash;
            }
        }

        private int HashValue(int seed, object value)
        {
            var currentHash = value != null
                       ? value.GetHashCode()
                       : 0;

            return seed * 37 + currentHash;
        }
    }


}
