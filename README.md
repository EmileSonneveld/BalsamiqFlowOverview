BalsamiqFlowOverview
====================

This tool generates an overview of the control flow from a Balsamiq document.
It reads the Balsamiq file and generates an SVG (and a Grapviz file) that show how the user can navigate between screens of the Balsamiq mockup.

[![cover_photo](https://github.com/EmileSonneveld/BalsamiqFlowOverview/blob/master/example_grapviz.svg)](https://raw.githubusercontent.com/EmileSonneveld/BalsamiqFlowOverview/master/example_grapviz.svg)

Check out the releases tab on GitHub to download the command line version.

Run the tool like this:

```BalsamiqFlowOverview.exe /path/to/balsamiq.bmpr [-graphviz]```

This will output a 'flow_graph.svg' file in the current directory.
"Use -graphviz to also output the graphviz notation: 'flow_graphviz.txt'
