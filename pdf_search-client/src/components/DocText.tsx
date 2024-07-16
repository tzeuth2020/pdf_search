import { useEffect, useState } from "react";
import config from "../config.json";

interface DocTextProps {
    group: string | undefined;
    name: string | undefined;
    question: number;
    pattern: string;
}


export const DocText: React.FC<DocTextProps> = ({ group, name, question, pattern}) => {
    const wholeDocRoute = `http://${config.server_host}:${config.server_port}/Submission/GetSubmission?group=${group}&name=${name}`;
    const questionRoute = `http://${config.server_host}:${config.server_port}/Submission/GetQuestion?group=${group}&name=${name}&question=${question}`;
    const indicesRoute = 
        `http://${config.server_host}:${config.server_port}/Search/FindMatches?group=${group}&name=${name}&question=${question}&pattern=${pattern}&limit=${Math.floor(pattern.length / 3)}`;

    
    const [docText, setDocText] = useState("");
    const [textBasic, setTextBasic] = useState<string[]>([]);
    const [textHighlight, setTextHighlight] = useState<string[]>([]);
    const [indices, setIndices] = useState<number[]>([]);

    useEffect(() => {
        const fetchWholeDoc = async () => {
            await fetch(wholeDocRoute)
                .then(response => response.text())
                .then(text => {
                    setDocText(text);
                })
                .catch(error => {
                    console.error('Error fetching document:', error);
                });
        }

        const fetchQuestiontext = async () => {
            await fetch(questionRoute)
                .then(response => response.text())
                .then(text => {
                    setDocText(text);
                })
                .catch(error => {
                    console.error('Error fetching question:', error);
                });
        }
        if (group && name && question === 0) {
            fetchWholeDoc();
        } else if (group && name) {
            fetchQuestiontext();
        }
    }, [wholeDocRoute, questionRoute, question]);


    useEffect(() => {
        const fetchIndices = async () => {
            await fetch(indicesRoute)
                .then(response => response.text())
                .then(text => text !== "" && text !== "[]" ?
                    text.substring(1, text.length - 1).split(',').map(str=> parseInt(str.trim())) : [])
                .then(list => list.length > 0 && isNaN(list[0]) ? setIndices([]) : setIndices(list))
                .catch(error => {
                    console.error('Error fetching document:', error);
                });
        }

        if (pattern !== "" && name !== undefined) {
            fetchIndices();
        } else {
            setIndices([]);
        }
    }, [indicesRoute, pattern, group, name]);

    useEffect(() => {
        if (indices.length > 0 && !isNaN(indices[0])) {
            var i : number = 0;
            var basicText : string[] = [];
            var highlightText: string[] = [];
            var currIndex: number = 0;
            while (i < docText.length) {
                if (currIndex === indices.length) {
                    basicText.push(docText.substring(i));
                    break;
                }
                if (i != indices[currIndex]) {
                    basicText.push(docText.substring(i, indices[currIndex]))
                    i = indices[currIndex];
                } else {
                    highlightText.push(docText.substring(i, i + pattern.length));
                    i += pattern.length;
                    currIndex++;
                }
            }
            setTextBasic(basicText);
            setTextHighlight(highlightText);
        } else {
            setTextBasic([docText]);
            setTextHighlight([]);
        }
    }, [indices, docText]);

    return(
        <div>
            <div>
                {textBasic.map((text, index) => {
                    return (<span key={index}> {text} 
                    {index < textHighlight.length && 
                    <span className="bg-yellow-500">{textHighlight[index]}</span>
                    }
                    </span>)})}
            </div>
            <div>
                {indices && indices.map((text, index) => <p key={index}>{text}</p>)}
            </div>
        </div>
        
    )

}