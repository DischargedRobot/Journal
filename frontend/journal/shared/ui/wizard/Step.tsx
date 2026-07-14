import Box from "@mui/material/Box"
import Stack from "@mui/material/Stack"
import {
	Children,
	cloneElement,
	isValidElement,
	ReactElement,
	ReactNode,
} from "react"
import { useWizard } from "./Wizard"

type StepChildProps = {
	stepId?: number | string
	children?: ReactNode
}

export const Step = ({
	children,
	stepId,
}: {
	children?: ReactNode
	stepId?: number | string
}) => {
	return (
		<>
			{Children.map(children, (child) => {
				if (!isValidElement(child)) {
					return child
				}

				if (child.type === StepHeader || child.type === StepContent) {
					return cloneElement(child as ReactElement<StepChildProps>, {
						stepId,
					})
				}

				return child
			})}
		</>
	)
}

export const Label = ({ children }: { children: ReactNode }) => {
	return <span>{children}</span>
}

export const StepContent = ({
	children,
	stepId,
}: {
	children: ReactNode
	stepId?: number | string
}) => {
	const { currentStep } = useWizard()

	return (
		<Stack
			className="flex-1 py-5 px-7"
			sx={{
				gridArea: "1 / 1",
				visibility: stepId === currentStep ? "visible" : "hidden",
				pointerEvents: stepId === currentStep ? "auto" : "none",
			}}
		>
			{children}
		</Stack>
	)
}

export const StepHeader = ({
	children,
	stepId,
}: {
	children: ReactNode
	stepId?: number | string
}) => {
	const { currentStep, setCurrentStep } = useWizard()

	return (
		<Stack
			className="cursor-pointer items-center"
			direction="row"
			spacing={2}
			onClick={() => {
				if (stepId !== undefined) {
					setCurrentStep(stepId)
				}
			}}
		>
			<Box
				className="rounded-full p-2"
				sx={{
					textAlign: "center",
					width: "2.5rem",
					height: "2.5rem",
					backgroundColor:
						stepId === currentStep
							? "primary.main"
							: "secondary.main",
					color:
						stepId === currentStep
							? "primary.contrastText"
							: "secondary.contrastText",
				}}
			>
				{stepId}
			</Box>
			<Label>{children}</Label>
		</Stack>
	)
}
