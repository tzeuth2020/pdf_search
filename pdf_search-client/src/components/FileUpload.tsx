import { useState } from "react";
import config from "../config.json";

interface UploadBoxProps {
    groupNames: string[];
    fetchGroupNames: () => void;
}

export const UploadBox: React.FC<UploadBoxProps> = ({groupNames, fetchGroupNames}) => { 
    const [files, setFiles] = useState<FileList | null>(null);
    const [status, setStatus] = useState<'initial' | 'uploading' | 'success' | 'fail'>('initial');
    const [group, setGroup] = useState<string | null>(null);
    const [message, setMessage] = useState<string>("");
    const [showNewGroup, setShowNewGroup] = useState<boolean>(false);
    const [submitClicked, setSubmitClicked] = useState<boolean>(false);
    
    
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setStatus('initial');
            setFiles(e.target.files);
        }
    }

    const handleUpload = async() => {
        setSubmitClicked(true);
        if (files && group) {
            setStatus('uploading');

            Array.from(files).forEach(async (file) => {
                const formData = new FormData();
                formData.append("uploadFile", file);
                const uploadURL : string = `http://${config.server_host}:${config.server_port}/Submission/Upload?group=${group}`
                await fetch(uploadURL, {
                    method: 'POST',
                    body: formData,
                }).then(res => res.text())
                .then((text) => setMessage(text))
                .catch(error => {
                    console.error('Error fetching names:', error);
                    setStatus('fail');
                })
                await fetchGroupNames();
            });          
        } else if (group) {
            setMessage("Please select files.")
        }   else {
            setMessage("Please select group.")
        }   
    }

    const handleSelectChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const value = event.target.value;
        setGroup(value);
        setShowNewGroup(value === "new");
    }

    return (
        <>
            <div className = "input-group">
                <label htmlFor="dropdown">Select a group:</label>
                <select id="dropdown" value={group ?? ""} onChange={handleSelectChange}>
                    {groupNames.map((name, index) => <option value={name} key={index}>{name}</option> )}
                    <option value="new" >New...</option>
                </select>
                {showNewGroup && (
                    <input
                        type="text"
                        placeholder="New Group ..."
                        value = {group ?? ""}
                        onChange  = {(e) => (setGroup(e.target.value))}
                    />
                )}
                <p>Selected Group: {group}</p>
            </div>
            <div className = "input-group">
                <label htmlFor="file" className="sr-only">
                    Choose Files
                </label>
                <input id="file" type="file" multiple onChange={handleFileChange}></input>
            </div>
            <div>
                {files && (
                    Array.from(files).map((file, index) => <p key={index}> {file.name}</p>)
                )}
                <button onClick={handleUpload}>Submit</button>
                {submitClicked &&  <p>{message}</p>}
            </div>
        </>
    )
}