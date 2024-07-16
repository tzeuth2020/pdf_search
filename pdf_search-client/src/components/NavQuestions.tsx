import { useState } from "react";
import { useEffect } from "react";
import config from "../config.json";

interface NavQuestionsProps {
    group: string;
    name: string;
    setActiveQuestion: (question: number) => void;
}

export const NavQuestions: React.FC<NavQuestionsProps> = ({ group, name, setActiveQuestion }) => {
    const[submissionQuestions, setSubmissionQuestions] = useState([]);
    const questionsRoute = 
        `http://${config.server_host}:${config.server_port}/Submission/GetFilledQuestions?group=${group}&name=${name}`;

    useEffect(() =>  {
    const fetchQuestions = async () => {
        await fetch(questionsRoute)
        .then(response => response.json())
        .then(data => {
            setSubmissionQuestions(data);
        })
        .catch(error => {
            console.error('Error fetching questions:', error);
        });
    }
        fetchQuestions();
    }, [questionsRoute]);

    return (
        <ul>
            {submissionQuestions.map((name, index) => 
            <li key={index}> 
                <button onClick = {() => setActiveQuestion(name)}> {name} </button>
            </li>)}
        </ul>
    )
}