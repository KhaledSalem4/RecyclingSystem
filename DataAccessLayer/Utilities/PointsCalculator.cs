using DataAccessLayer.Entities;

namespace DataAccessLayer.Utilities
{
    public static class PointsCalculator
    {
        // Points per kilogram for each material type
        private static readonly Dictionary<MaterialType, int> PointsPerKilo = new()
        {
            { MaterialType.Plastic, 5 },
            { MaterialType.Can, 10 },
            { MaterialType.Carton, 8 },
            { MaterialType.Glass, 7 }
        };

        /// <summary>
        /// Calculates points for a given material type and weight in kilograms
        /// </summary>
        public static int CalculatePoints(MaterialType materialType, double weightInKg)
        {
            if (weightInKg <= 0)
                return 0;

            if (!PointsPerKilo.ContainsKey(materialType))
                return 0;

            return (int)(PointsPerKilo[materialType] * weightInKg);
        }

        /// <summary>
        /// Calculates points based on material type name string
        /// </summary>
        public static int CalculatePoints(string materialTypeName, double weightInKg)
        {
            if (string.IsNullOrWhiteSpace(materialTypeName) || weightInKg <= 0)
                return 0;

            if (Enum.TryParse<MaterialType>(materialTypeName, true, out var materialType))
            {
                return CalculatePoints(materialType, weightInKg);
            }

            return 0;
        }

        /// <summary>
        /// Calculates total points for a collection of materials
        /// </summary>
        public static int CalculateTotalPoints(IEnumerable<Material> materials)
        {
            if (materials == null || !materials.Any())
                return 0;

            return materials.Sum(m => CalculatePoints(m.TypeName ?? string.Empty, m.Size));
        }

        /// <summary>
        /// Calculates total points for an order (NEW SYSTEM)
        /// </summary>
        public static int CalculateOrderPoints(Order order)
        {
            if (order == null)
                return 0;

            return CalculatePoints(order.TypeOfMaterial, order.Quantity);
        }
    }
}