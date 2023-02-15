import "./App.css";
import React from "react";
import BranchSelector from "./components/BranchSelector";
import CommitCards from "./components/CommitCards";
import initApi from "./api";

const API_URL = `https://api.github.com/repos/tryphotino/photino.NET/commits?per_page=3&sha=`;

class App extends React.Component {
    constructor(props) {
        super(props);

        this.branches = ["master", "debug"];
        this.state = {
            currentBranch: undefined,
            commits: [],
        };

        this.selectBranch = this.selectBranch.bind(this);
        this.testMessageClicked = this.testMessageClicked.bind(this);
    }

    componentWillMount() {
        this.selectBranch(this.branches[0]);
        initApi();
    }

    async selectBranch(branch) {
        this.setState({ currentBranch: branch });
        this.setState({ commits: await this.fetchCommits(branch) });
    }

    async fetchCommits(branch) {
        const url = `${API_URL}${branch}`;
        return await (await fetch(url)).json();
    }

    render() {
        return (
            <div id="content">
                <h1>Latest Photino.NET Commits</h1>
                <BranchSelector
                    options={this.branches}
                    value={this.state.currentBranch}
                    onChange={async (branch) => await this.selectBranch(branch)}
                />
                <p>tryphotino/photino.NET @{this.state.currentBranch}</p>
                <CommitCards commits={this.state.commits} />
                <button onClick={this.testMessageClicked}>Test Message</button>
                <div id="output"></div>
            </div>
        );
    }

    testMessageClicked(){
        
        alert("i got this!");
//         message = {};
//   3         message.Command = "getUserProfile";
//   4         message.Parameters = "";
    
//   5        let sMessage = JSON.stringify(message);
//   6      console.log(sMessage);
        window.external.sendMessage(JSON.stringify({Command:"getUserProfile",Parameters:""}));
    }
}

export default App;
