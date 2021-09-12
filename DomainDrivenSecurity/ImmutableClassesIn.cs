using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Temp {
    class Student {
        public string Name { get; }
        public int RegNo { get; }
        public IImmutableDictionary<string, string> Metadata { get; }

        public Student(string name, int regNo,
                       IDictionary<string, string> metadata) {
            Name = name;
            RegNo = regNo;
            Metadata = ImmutableDictionary.ToImmutableDictionary(metadata);
        }
    }

    class OldStudent {
        public string Name { get; set; }
        public int RegNo { get; set; }
        public IDictionary<string, string> Metadata { get; set; }

        public OldStudent(string name, int regNo,
                          IDictionary<string, string> metadata) {
            Name = name;
            RegNo = regNo;
            Metadata = metadata;
        }
    }

    namespace Tests {
        [TestClass]
        public class StudentTests {
            [TestMethod]
            public void Create_WithData_Works() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new Student("Johan", 1234, metadata);

                // Assert
                var clonedMetadata = student.Metadata;
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsTrue(clonedMetadata.ContainsKey("Company"));
                Assert.IsTrue(clonedMetadata["Company"] == "Truesec");
            }

            [TestMethod]
            public void CreateOldStudent_WithDataAndChangeLocal_Works() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new OldStudent("Johan", 1234, metadata);
                metadata["City"] = "Stockholm";
                metadata.Remove("Company");

                // Assert
                var clonedMetadata = student.Metadata;
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Stockholm");
                Assert.IsFalse(clonedMetadata.ContainsKey("Company"));
            }

            [TestMethod]
            public void Create_WithDataAndChangeLocal_Works() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new Student("Johan", 1234, metadata);
                metadata["City"] = "Stockholm";
                metadata.Remove("Company");

                // Assert
                var clonedMetadata = student.Metadata;
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsTrue(clonedMetadata.ContainsKey("Company"));
                Assert.IsTrue(clonedMetadata["Company"] == "Truesec");
            }

            [TestMethod]
            public void CreateOldStudent_WithDataAndChangeLocal_Possible() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new OldStudent("Johan", 1234, metadata);
                student.Metadata.Remove("Company"); // Disappears

                // Assert
                var clonedMetadata = student.Metadata;
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsFalse(clonedMetadata.ContainsKey("Company"));
            }

            [TestMethod]
            public void Create_WithDataAndChangeLocal_NotPossible() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new Student("Johan", 1234, metadata);
                student.Metadata.Remove("Company"); // Nothing happens

                // Assert
                var clonedMetadata = student.Metadata;
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsTrue(clonedMetadata.ContainsKey("Company"));
                Assert.IsTrue(clonedMetadata["Company"] == "Truesec");
            }

            [TestMethod]
            public void CreateOldStudent_WithDataAndChangeLocalData_Possible() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new OldStudent("Johan", 1234, metadata);
                var clonedMetadata = student.Metadata;
                clonedMetadata.Remove("Company"); // Should disappear

                // Assert
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsFalse(clonedMetadata.ContainsKey("Company"));
            }

            [TestMethod]
            public void Create_WithDataAndChangeLocalData_NotPossible() {
                // Arrange
                var metadata = new Dictionary<string,string>();
                metadata["City"] = "Vallentuna";
                metadata["Company"] = "Truesec";

                // Act
                var student = new Student("Johan", 1234, metadata);
                var clonedMetadata = student.Metadata;
                clonedMetadata.Remove("Company"); // Nothing happens

                // Assert
                Assert.IsTrue(clonedMetadata.ContainsKey("City"));
                Assert.IsTrue(clonedMetadata["City"] == "Vallentuna");
                Assert.IsTrue(clonedMetadata.ContainsKey("Company"));
                Assert.IsTrue(clonedMetadata["Company"] == "Truesec");
            }
        }
    }
}
