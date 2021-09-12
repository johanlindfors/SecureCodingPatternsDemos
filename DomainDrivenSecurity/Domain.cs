using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain {
    namespace OldModels {

        public class Book {
            public string ISBN { get; set; }
            // more code here

            public Book(string isbn) {
                this.ISBN = isbn;
            }
        }
    }

    namespace Models {
        using Primitives;

        public class Book {
            public ISBN ISBN { get; }
            // more code here

            private Book() { }

            public override string ToString() {
                return this.ISBN.ToString();
            }

            public Book(string isbn) {
                this.ISBN = new ISBN(isbn);
            }
        }
    }

    namespace Primitives {

        public struct ISBN {
            private readonly string isbn;

            public ISBN(string isbn) {
                if(!IsValid(isbn)) {
                    throw new ArgumentException("Invalid ISBN.");
                }
                this.isbn = TrimToDigits(isbn);
            }

            public readonly override string ToString() {
                return this.isbn;
            }

#region Validation
            private static bool IsValid(string isbn) {
                return !string.IsNullOrEmpty(isbn) &&
                       Has10Or13Digits(isbn) &&
                       IsValidISBNPattern(isbn) &&
                       HasValidChecksum(isbn);
            }

            // Pattern used from https://howtodoinjava.com/java/regex/java-regex-validate-international-standard-book-number-isbns/
            private const string ISBN_PATTERN = "^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$";

            private static bool IsValidISBNPattern(string isbn) {
                Regex rgx = new Regex(ISBN_PATTERN);
                return rgx.IsMatch(isbn);
            }

            private static bool Has10Or13Digits(string isbn) {
                var trimmedIsbn = TrimToDigits(isbn);
                return trimmedIsbn.Length == 10 ||
                       trimmedIsbn.Length == 13;
            }

            private static bool HasValidChecksum(string isbn) {
                var trimmedIsbn = TrimToDigits(isbn);
                return ISBN10ChecksumValid(trimmedIsbn) ||
                       ISBN13ChecksumValid(trimmedIsbn);
            }

            private static bool ISBN10ChecksumValid(string isbn) {
                int sum = 0;
                int index = 10;
                foreach(char digit in isbn) {
                    if(Char.IsDigit(digit)){
                        sum += (int)digit * index--;
                    }
                }
                return sum % 11 == 0;
            }

            private static bool ISBN13ChecksumValid(string isbn) {
                int sum = 0;
                int index = 13;
                foreach(char digit in isbn) {
                    if(Char.IsDigit(digit)){
                        sum += (int)digit * (index % 2 == 0 ? 3 : 1);
                        index--;
                    }
                }
                return sum % 10 == 0;
            }
#endregion

            // Canocalization
            public static string TrimToDigits(string isbn) {
                if(isbn.StartsWith("ISBN-")) {
                    isbn = isbn.Remove(5,2);
                }
                return new string(isbn.Where(c => Char.IsDigit(c)).ToArray());
            }
        }
    }

    namespace OldTests {
        using OldModels;

        [TestClass]
        public class BookTests {
            [DataTestMethod]
            [DataRow("0-596-52068-9")]
            [DataRow("ISBN 978-0-596-52068-7")]
            [DataRow("ISBN-13: 978-0-596-52068-7")]
            [DataRow("978 0 596 52068 7")]
            [DataRow("9780596520687")]
            [DataRow("0-596-52068-9")]
            [DataRow("0 596 52068 9")]
            [DataRow("ISBN-10 0-596-52068-9")]
            [DataRow("ISBN-10: 0-596-52068-9")]
            public void Create_WithValidISBN_ShouldSucceed(string isbn) {
                // Arrange
                // Act
                var book = new Book(isbn);

                // Assert
                Assert.AreEqual(isbn, book.ISBN);
            }

            // [DataTestMethod]
            // [DataRow(null)]
            // [DataRow("")]
            // [DataRow("ISBN 11978-0-596-52068-7")]
            // [DataRow("ISBN-12: 978-0-596-52068-7")]
            // [DataRow("978 10 596 52068 7")]
            // [DataRow("119780596520687")]
            // [DataRow("0-5961-52068-9")]
            // [DataRow("11 5122 52068 9")]
            // [DataRow("ISBN-11 0-596-52068-9")]
            // [DataRow("ISBN-10- 0-596-52068-9")]
            // [DataRow("0 512 52068 9")]
            // public void Create_WithInvalidISBN_ShouldFail(string isbn) {
            //     // Arrange
            //     // Act
            //     var book = new Book(isbn);

            //     // Assert
            //     Assert.AreEqual(isbn, book.ISBN);
            // }
        }
    }

    namespace Tests {
        using Models;
        using Primitives;

        [TestClass]
        public class BookTests {

            [TestMethod]
            public void Create_BookWithoutISBN_ShouldNotWork() {
                // Arrange
                Book book;

                // Act
                //var bookAsString = book.ToString();

                // Assert
            }

            [DataTestMethod]
            [DataRow("0-596-52068-9")]
            [DataRow("ISBN 978-0-596-52068-7")]
            [DataRow("ISBN-13: 978-0-596-52068-7")]
            [DataRow("978 0 596 52068 7")]
            [DataRow("9780596520687")]
            [DataRow("0-596-52068-9")]
            [DataRow("0 596 52068 9")]
            [DataRow("ISBN-10 0-596-52068-9")]
            [DataRow("ISBN-10: 0-596-52068-9")]
            public void Create_WithValidISBN_ShouldSucceed(string isbn) {
                // Arrange
                // Act
                var book = new Book(isbn);

                // Assert
                Assert.AreEqual(book.ISBN.ToString(), ISBN.TrimToDigits(isbn));
            }

            [DataTestMethod]
            [DataRow(null)]
            [DataRow("")]
            [DataRow("ISBN 11978-0-596-52068-7")]
            [DataRow("ISBN-12: 978-0-596-52068-7")]
            [DataRow("978 10 596 52068 7")]
            [DataRow("119780596520687")]
            [DataRow("0-5961-52068-9")]
            [DataRow("11 5122 52068 9")]
            [DataRow("ISBN-11 0-596-52068-9")]
            [DataRow("ISBN-10- 0-596-52068-9")]
            [DataRow("0 512 52068 9")]
            [ExpectedException(typeof(ArgumentException))]
            public void Create_WithInvalidISBN_ShouldFail(string isbn) {
                // Arrange
                // Act
                var book = new Book(isbn);

                // Assert
            }
        }

        [TestClass]
        public class ISBNTests {

            [DataTestMethod]
            [DataRow(null)]
            [DataRow("")]
            [DataRow("ISBN 11978-0-596-52068-7")]
            [DataRow("ISBN-12: 978-0-596-52068-7")]
            [DataRow("978 10 596 52068 7")]
            [DataRow("119780596520687")]
            [DataRow("0-5961-52068-9")]
            [DataRow("11 5122 52068 9")]
            [DataRow("ISBN-11 0-596-52068-9")]
            [DataRow("ISBN-10- 0-596-52068-9")]
            [DataRow("0 512 52068 9")]
            [ExpectedException(typeof(ArgumentException))]
            public void Create_WithEmptyString_ShouldFail(string isbn) {
                // Arrange
                // Act
                var ISBN = new ISBN(isbn);

                // Assert
            }

            [DataTestMethod]
            [DataRow("0-596-52068-9")]
            [DataRow("ISBN 978-0-596-52068-7")]
            [DataRow("ISBN-13: 978-0-596-52068-7")]
            [DataRow("978 0 596 52068 7")]
            [DataRow("9780596520687")]
            [DataRow("0-596-52068-9")]
            [DataRow("0 596 52068 9")]
            [DataRow("ISBN-10 0-596-52068-9")]
            [DataRow("ISBN-10: 0-596-52068-9")]
            public void Create_WithValidISBN_ShouldSucceed(string isbn) {
                // Arrange
                // Act
                var ISBN = new ISBN(isbn);

                // Assert
                Assert.AreEqual(ISBN.ToString(), ISBN.TrimToDigits(isbn));
            }
        }
    }
}
