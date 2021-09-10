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

            public Book(string isbn) {
                this.ISBN = new ISBN(isbn);
            }
        }
    }

    namespace Primitives {

        public struct ISBN {
            private readonly string isbn;

            public ISBN(string isbn) {
                // not null
                IsNotNullOrEmpty(isbn);
                // 10 or 13 digits
                Has10Or13Digits(isbn);
                // valid pattern
                IsValidISBNPattern(isbn);
                // valid checksum
                HasValidChecksum(isbn);

                this.isbn = TrimToDigits(isbn);
            }

            public readonly override string ToString() {
                return this.isbn;
            }

#region Validation
            private static void IsNotNullOrEmpty(string isbn) {
                if(string.IsNullOrEmpty(isbn)) 
                    throw new ArgumentException("Null or empty initial value to isbn.");
            }

            // Pattern used from https://howtodoinjava.com/java/regex/java-regex-validate-international-standard-book-number-isbns/
            private const string ISBN_PATTERN = "^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$";

            private static void IsValidISBNPattern(string isbn) {
                Regex rgx = new Regex(ISBN_PATTERN);
                if(!rgx.IsMatch(isbn))
                    throw new ArgumentException($"Pattern of '{isbn} isn't valid.");
            }

            private static void Has10Or13Digits(string isbn) {
                var trimmedIsbn = TrimToDigits(isbn);

                if(!(trimmedIsbn.Length == 10 || trimmedIsbn.Length == 13)) 
                    throw new ArgumentException($"Count of digits in '{isbn}' is {trimmedIsbn.Length}.");
            }

            private static void HasValidChecksum(string isbn) {
                var trimmedIsbn = TrimToDigits(isbn);

                if(!ISBN10ChecksumValid(trimmedIsbn) && !ISBN13ChecksumValid(trimmedIsbn)) 
                    throw new ArgumentException($"Checksum of '{isbn}' is invalid.");
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
