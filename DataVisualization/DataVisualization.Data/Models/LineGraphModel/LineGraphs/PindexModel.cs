﻿using System;

namespace DataVisualization.Data.Models.LineGraphModel.LineGraphs {
    public class PindexModel {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FirstAvailableDate { get; set; }
        public DateTime LastAvailableDate { get; set; }
    }
}
