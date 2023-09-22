using LcrSimulator.Core;

namespace LcrSimulator.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PlayerCountFail()
        {
            var simulator = new ArraySimulator();
            Assert.Throws(typeof(ArgumentException), () => simulator.Simulate(1, 3));
        }

        [Test]
        public void GameCountFail()
        {
            var simulator = new ArraySimulator();
            Assert.Throws(typeof(ArgumentException), () => simulator.Simulate(3, 0));
        }

        [Test]
        public void Simulate()
        {
            var simulator = new ArraySimulator();
            var result = simulator.Simulate(100, 100_000);

            Assert.That(result.Shortest, Is.GreaterThan(0));
            Assert.That(result.Longest, Is.GreaterThan(0));
            Assert.That(result.MostWins, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.Average, Is.GreaterThan(0));
        }
    }
}