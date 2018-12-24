
export class Margin {
    top: number;
    right: number;
    bottom: number;
    left: number;
}

export class ChartOffsetRange {
    min: number;
    max: number;
}

export class BarDimensions {
    width: number;
    gap: number;
    maxWidth: number;
    minWidth: number;
}

export class ChartDimensions {
    margin: Margin;
    chartOffset: ChartOffsetRange;
    bars: BarDimensions;
    height: number;
    width: number;
}
