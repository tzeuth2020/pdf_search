import { useState } from "react";
import { useEffect } from "react";
import config from "../config.json";
import { NavQuestions } from "./NavQuestions";

interface NavNamesProps {
    group: string;
    activeName: string | undefined;
    setActiveName: (name: string) => void;
    setActiveQuestion: (question: number) => void;
}

export const NavNames: React.FC<NavNamesProps> = ({ group, activeName, setActiveName, setActiveQuestion }) => {
    const[submissionNames, setSubmissionNames] = useState([]);
    const namesRoute = `http://${config.server_host}:${config.server_port}/Submission/SubmissionNames?group=${group}`;
    const [activeIndex, setActiveIndex] = useState(0);

    useEffect(() =>  {
    const fetchNames = async () => {
        await fetch(namesRoute)
            .then(response => response.json())
            .then(data => {
                setSubmissionNames(data);
            })
        .catch(error => {
            console.error('Error fetching names:', error);
        });
    }
        fetchNames();
    }, [namesRoute]);

    return (
        <ul>
            {submissionNames.map((name, index) => 
            <li key={index}> 
                <button className= "pl-16 text-white text-sans text-lg w-full text-left hover:bg-lighterTeal" onClick = {() => {setActiveQuestion(0); setActiveName(name)}}> 
                    <span className="pl-4">{name}</span>
                </button>
                {(activeName !== undefined && activeName === name) && <NavQuestions group = {group} name={activeName} setActiveQuestion={setActiveQuestion}/>}
            </li>)}
        </ul>
    )
}