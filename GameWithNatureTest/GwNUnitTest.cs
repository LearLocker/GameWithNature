using NUnit.Framework;
using GameWithNature;

namespace GameWithNatureTest
{
    [TestFixture]
    public class Tests
    {
        Game gameWN = new Game();
        [Test]
        public void WaldTest()
        {
            Assert.AreEqual(1, gameWN.Wald());
        }

        [Test]
        public void HurwitzTest()
        {
            Assert.AreEqual(1, gameWN.Hurwitz(0.5));
        }

        [Test]
        public void SavageTest()
        {
            Assert.AreEqual(1, gameWN.Savage());
        }

        [Test]
        public void VerTest()
        {
            double[] testver = new double[] { 0.25, 0.25, 0.25, 0.25 };
            Assert.AreEqual(1, gameWN.Ver(testver));
        }

        [Test]
        public void GameMatrixTest()
        {
            double[,] TestGMatrix = { { 2, 7, 8, 6 }, { 2, 8, 7, 3 }, { 4, 3, 4, 2 } };
            Assert.AreEqual(TestGMatrix, gameWN.GetMatrix());
        }

        [Test]
        public void RiskMatrixTest()
        {
            double[,] TestRMatrix = { { 2, 1, 0, 0 }, { 2, 0, 1, 3 }, { 0, 5, 4, 4} };
            Assert.AreEqual(TestRMatrix, gameWN.GetRiskMatrix());
        }
    }
}