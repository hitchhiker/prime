import { OhlcRecord } from "./ohlc/ohlc-record";
import * as d3 from "d3";

class OhlcItems {
    records: OhlcRecord[];
    index: number;
}

export class Viewport {
    constructor(x1) {
        this.x1 = x1;
    }

    x1: number;
    x2: number;
}

export class ChartCore {
    private chartData: OhlcRecord[];
    private svg;
    private g;

    constructor(selector: string) {
        this.initialize(selector);
    }

    public sizing = {
        margin: {
            top: 10, right: 10, bottom: 10, left: 10
        },
        bars: {
            width: 10,
            gap: 2
        },
        height: 450,
        width: 600
    };

    private _viewport: Viewport = new Viewport(0);
    public set viewport(v: Viewport) {
        this._viewport.x1 = v.x1;
        this._viewport.x2 = v.x1 + this.sizing.width;
        console.log(this._viewport);
    }
    public get viewport(): Viewport {
        return this._viewport;
    }

    public setData(data: OhlcRecord[]) {
        this.viewport = { x1: 50, x2: 0 };
        this.chartData = data;

        //this.viewPort.x1 = this.getXbyIndex(data.length - 1);
    }

    public getXbyIndex(i: number): number {
        return i * (this.sizing.bars.width + this.sizing.bars.gap)
    }

    public getInView(data: OhlcRecord[], v: Viewport): OhlcItems {
        let startingIndex = -1;
        let inView = data.filter((record, i) => {
            let currX = this.getXbyIndex(i);
            let r = currX >= v.x1 && currX <= v.x2;

            if(r === true && startingIndex === -1) {
                startingIndex = i;
            }

            return r;
        });

        return { 
            index: startingIndex,
            records: inView
        };
    }

    public initialize(selector: string) {
        this.svg = d3.select(selector)
            .append("svg").attr("width", this.sizing.width).attr("height", this.sizing.height);

        this.svg.append("rect").attr("width", "100%").attr("height", "100%").attr("fill", "rgb(30, 30, 30)");
        this.g = this.svg.append("g");
    }

    public render() {
        let svg = this.svg;
        let allData = this.chartData;
        let sizing = this.sizing;
        let viewport = this.viewport;
        let g = this.g;

        let data = this.getInView(allData, viewport);

        let yScaleMin: number = d3.min(data.records, (d: OhlcRecord) => {
            return d.low;
        });
        let yScaleMax: number = d3.max(data.records, (d: OhlcRecord) => {
            return d.high;
        });

        // Scales.
        let yScaleRaw = d3.scaleLinear()
            .domain([yScaleMin, yScaleMax]) // // d3.extent(data, (x: OhlcRecord) => { return x.open; })
            .range([sizing.height, 0]);
        let yScale = function (v) {
            return yScaleRaw(v).toFixed(4);
        };

        // Clear everything.
        g.selectAll("*").remove();

        // Populate data.
        let svgData = g
            .selectAll("g")
            .data(data.records);

        // Add groups.
        let groups = svgData.enter().append("g")
            .attr("transform", (x: OhlcRecord, i) => {
                return `translate(${i * (sizing.bars.width + sizing.bars.gap)}, ${yScale(x.low)})`;
            });

        groups.append("line");
        groups.append("rect");

        // Axis.
        // let xAxis = d3.axisBottom(yScaleRaw);
        // svg.append("g").call(xAxis);

        //let gW = g.node().getBBox().width;

        // Zoom.
        // svg
        //   .call(d3.zoom()
        //     .scaleExtent([1 / 2, 4])
        //     .translateExtent([[-100, svgH / 2], [gW + 100, svgH / 2]])
        //     .on("zoom", zoomed));

        // function zoomed() {
        //   //console.log(d3.event.transform);
        // }

        // Line test.
        // function lineTest() {
        //     let line = d3.line()
        //         .x((d: OhlcRecord, i) => {
        //             return i * 10;
        //         })
        //         .y((da: OhlcRecord, i) => {
        //             return yScaleRaw(da.open);
        //         });
        //     svg.append("path")
        //         .attr("stroke-width", 1)
        //         .attr("stroke", "white")
        //         .attr("fill", "transparent")
        //         .attr("d", line(data));
        // }

        // Process lines.
        groups.selectAll("line")
            .attr("transform", (x: OhlcRecord, i) => {
                return `translate(${(sizing.bars.width / 2)}, ${0})`;
            })
            .attr("x1", 0).attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", (x: OhlcRecord, i) => {
                return (yScale(x.high) - yScale(x.low));
            })
            .attr("stroke-width", 1)
            .attr("stroke", (o: OhlcRecord) => {
                return getColor(o);
            });

        // Process rects.
        groups.selectAll("rect")
            .attr("width", sizing.bars.width)
            .attr("height", (o: OhlcRecord) => {
                return Math.abs(yScale(o.open) - yScale(o.close));
            })
            .attr("fill", (o: OhlcRecord) => {
                return getColor(o);
            })
            .attr("x", 0)
            .attr("y", (x: OhlcRecord, i) => {
                let openLow = yScale(x.open) - yScale(x.low);
                let closeLow = yScale(x.close) - yScale(x.low)
                return openLow < closeLow ? openLow : closeLow;
            });

        svgData.exit().remove();

        function getColor(o: OhlcRecord): string {
            return o.open - o.close >= 0 ? "red" : "green";
        }
    }
}
