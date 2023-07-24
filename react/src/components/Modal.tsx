import React, { ReactNode } from "react"
import { useModalControlContext } from "../ModalContext"

type ModalProps = {
    name: string
    icon: string
    onCancel?: () => void
    onAccept?: () => void
    children?: ReactNode
}

const Modal: React.FC<ModalProps> = ({
    children,
    name,
    icon,
    onCancel,
    onAccept,
}) => {
    const { closeModal } = useModalControlContext()

    return (
        <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-black text-white w-1/2 m-auto border-5 rounded-2xl shadow-sm shadow-slate-800">
            <div className="flex items-center gap-8 h-16">
                <span className="flex justify-center align-center ml-8">
                    <img src={icon} className="w-6" alt="Icon" />
                </span>
                <h1 className="text-3xl inline-block align-middle">{name}</h1>
            </div>
            <div className="mx-16 flex flex-col gap-8">{children}</div>
            <div className="flex justify-between mx-10 py-8">
                <input
                    type="button"
                    value="Cancel"
                    onClick={() => {
                        closeModal()
                        if (onCancel) onCancel()
                    }}
                    className="bg-red-500 rounded-md cursor-pointer px-4 py-1 text-black font-bold duration-100 hover:bg-red-600"
                />
                <input
                    type="button"
                    value="Accept"
                    onClick={() => {
                        closeModal()
                        if (onAccept) onAccept()
                    }}
                    className="bg-blue-500 rounded-md cursor-pointer px-4 py-1 text-black font-bold duration-100 hover:bg-blue-600"
                />
            </div>
        </div>
    )
}

export default Modal
